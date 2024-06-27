Merge commit: https://github.com/saulm314/code-rooms-exe/commit/312f831c45448f0d391ec84c6e4c2ea6596915a3

# Comment

This pull request gives us more accurate information about line number. Where a single statement spans multiple lines, it used to be that we could consider the top line as the line number, even if this top line consisted of only whitespace. Now, we have more information about which line the statement actually begins in (i.e. first non-whitespace character), and which a line a statement ends in.

The immediate benefit of this is that when an error is thrown, line numbers are reported more accurately. The benefit in the future will be that, should we eventually add GUI, we will be able to highlight a line that is currently executing or threw an error, without having to highlight any redundant whitespace lines around it.
