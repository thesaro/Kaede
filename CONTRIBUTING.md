These following points illustrate the base outline of the project:

* We're using MVVM design pattern and principles and will extensively use the `CommunityToolkit.Mvvm`
package to facilitate our progress. So get familiar with that. See [MSDN's MVVM Description](https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm),
and [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/MVVM-Samples)

* Dark themed based UI is the basis. This is non-negotiable. We may later add light/dark mode toggle but honestly
doing that in WPF is like arguing with your father so I don't think we'll bother.

* I have not added testing infrastructure yet but when we do, you're better running a `dotnet test`
before commiting to ensure you aren't breaking anything. If you have made changes that invalidate on tests,
you should explain why the particular test(s) shall not hold. For example "We're adding support for
usernames containing special characters so we need to change/remove this test that fails on special
characters on username field". See [dotnet test](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-test?tabs=dotnet-test-with-vstest).
* Code formatting should be according to the [MSDN's Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
(Just follow the Visual Studio's automatic formatting and you should be good).


If your code fails to hold any of these invariants, I will happily revert your changes
and probably roast you afterwards lol.
