# Model Binding For Matrix

A concise way of model binding for martix (2-rank array) in ASP.NET Core.

## Example

https://pathfinding-lab.codedwith.fun/?o=2,[2-4,6];[2,4-6],1;1,5

If the type of parameter `o` is set to `int` and the value (if present) is configured to 1, the parameter represents the following array:

```csharp
var o = new [] 
{
    new [] { 0, 0, 0, 0, 0, 0, 0 }, // Y = 0
    new [] { 0, 1, 0, 1, 1, 1, 0 }, // Y = 1
    new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 2
    new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 3
    new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 4
    new [] { 0, 1, 0, 0, 0, 0, 0 }, // Y = 5
    new [] { 0, 0, 1, 0, 0, 0, 0 }, // Y = 6
};
```

The library can also convert the array above in the format of parameter `o`.
