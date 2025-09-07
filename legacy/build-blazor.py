import http.server
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

def build(env):
    if env == "local":
        # in a live deployment, we don't need to remove these as we are already in a fresh environment
        print("Removing existing published data...")
        shutil.rmtree(path("code-rooms-exe"), ignore_errors=True)
    
    print("Publishing CREBlazorApp...")
    process_resp = subprocess.run(["dotnet", "publish", path("CREBlazorApp"), "-c", "Release", "-o", path("publish")])
    if process_resp.returncode != 0:
        print("Dotnet build failed")
        return False

    print("Moving published data...")
    # we do not remove anything from the publish directory so that for a local build,
    #   dotnet can use caching to drastically speed up the publishing process
    shutil.copytree(path("publish/wwwroot"), path("code-rooms-exe/__blazor/wwwroot"))
    shutil.move(path("code-rooms-exe/__blazor/wwwroot/_framework"), path("code-rooms-exe/_framework"))
    shutil.move(path("code-rooms-exe/__blazor/wwwroot/index.html"), path("code-rooms-exe/index.html"))
    shutil.move(path("code-rooms-exe/__blazor/wwwroot/404.html"), path("code-rooms-exe/404.html"))
    
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
    build_resp = build(env)
    if not build_resp:
        sys.exit(1)
    if env == "local":
        run_web_server()