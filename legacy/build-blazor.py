import http.server
import os
import os.path
import shutil
import subprocess
import sys

PORT = 8080

def get_env(args):
    if len(args) >= 2 and args[1] == "deploy":
        env = "deploy"
    else:
        env = "local"
    print(f"environment: {env}")
    return env

def add_cs_sources(env):
    if env == "local":
        print("Removing existing generated code...")
        shutil.rmtree(path("CREBlazorApp/GeneratedCode"), ignore_errors=True)
    
    print("Adding generated code...")
    os.mkdir(path("CREBlazorApp/GeneratedCode"))

    print("Adding GeneratedImageSources.cs...")
    with open(path("CREBlazorApp/GeneratedCode/GeneratedImageSources.cs"), "x") as image_sources_cs:
        image_sources_cs.write("""namespace CREBlazorApp.GeneratedCode;

public static class GeneratedImageSources
{
    public static readonly string[] ImageSources =
    [
""")
        for image_source in os.listdir(path("Files/Types")):
            image_sources_cs.write(f"        \"{image_source}\",\n")
        image_sources_cs.write("""    ];
}""")
    
    print("Adding GeneratedLevels.cs...")
    with open(path("CREBlazorApp/GeneratedCode/GeneratedLevels.cs"), "x") as generated_levels_cs:
        files = sorted(os.listdir(path("Files/Levels")))
        generated_levels_cs.write("""namespace CREBlazorApp.GeneratedCode;

public static class GeneratedLevels
{
    public static readonly string[] JsonLevels =
    [
""")
        for file in files:
            if not(file.endswith(".json")):
                continue
            generated_levels_cs.write('"""\n')
            with open(path(f"Files/Levels/{file}")) as json_level:
                generated_levels_cs.write(json_level.read())
            generated_levels_cs.write('\n""",\n')
        generated_levels_cs.write("""    ];

    public static readonly string[] Descriptions =
    [
""")
        for file in files:
            if not(file.endswith(".txt")):
                continue
            generated_levels_cs.write('"""\n')
            with open(path(f"Files/Levels/{file}")) as description:
                generated_levels_cs.write(description.read())
            generated_levels_cs.write('\n""",\n')
        generated_levels_cs.write("""    ];

    public static readonly string[] Solutions =
    [
""")
        for file in files:
            if not(file.endswith(".cs")):
                continue
            generated_levels_cs.write('"""\n')
            with open(path(f"Files/Levels/{file}")) as solution:
                generated_levels_cs.write(solution.read())
            generated_levels_cs.write('\n""",\n')
        generated_levels_cs.write("""    ];
}""")  

def build(env):
    if env == "local":
        # in a live deployment, we don't need to remove these as we are already in a fresh environment
        print("Removing existing published data...")
        shutil.rmtree(path("code-rooms-exe"), ignore_errors=True)
    
    print("Publishing CREBlazorApp...")
    process_resp = subprocess.run(["dotnet", "publish", path("CREBlazorApp"), "-c", "CREBlazor", "-o", path("publish")])
    if process_resp.returncode != 0:
        print("Dotnet build failed")
        return False

    print("Moving published data...")
    # we do not remove anything from the publish directory so that for a local build,
    #   dotnet can use caching to drastically speed up the publishing process
    shutil.copytree(path("publish/wwwroot"), path("code-rooms-exe/__blazor/wwwroot"))
    shutil.move(path("code-rooms-exe/__blazor/wwwroot/_framework"), path("code-rooms-exe/_framework"))
    shutil.move(path("code-rooms-exe/__blazor/wwwroot/index.html"), path("code-rooms-exe/index.html"))
    shutil.copy(path("code-rooms-exe/index.html"), path("code-rooms-exe/404.html"))

    print("Moving static files...")
    shutil.copytree(path("Files"), path("code-rooms-exe/Files"))
    shutil.rmtree(path("code-rooms-exe/Files/Save"))
    shutil.rmtree(path("code-rooms-exe/Files/Tests"))
    os.remove(path("code-rooms-exe/Files/Dummy.cs"))
    os.remove(path("code-rooms-exe/Files/Dummy.java"))
    
    print("Build done.")
    return True

def path(p):
    return os.path.normpath(p)

def run_web_server():
    try:
        with http.server.ThreadingHTTPServer(("", PORT), http.server.SimpleHTTPRequestHandler) as httpd:
            with open(path("code-rooms-exe/404.html")) as html_file:
                httpd.RequestHandlerClass.error_message_format = html_file.read()
            print(f"Serving at http://127.0.0.1:{PORT}/code-rooms-exe/ (Ctrl+C to stop)")
            httpd.serve_forever()
    except KeyboardInterrupt:
        print("Keyboard interrupt detected")

if __name__ == "__main__":
    env = get_env(sys.argv)
    add_cs_sources(env)
    build_resp = build(env)
    if not build_resp:
        sys.exit(1)
    if env == "local":
        run_web_server()