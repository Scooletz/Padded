# Padded
Fody AddIn that allows declarative padding structures and classes to fight the false sharing problem.

## How to
In your project add the Fody plugin published on nuget [here](https://www.nuget.org/packages/Padded.Fody/) and create one class with the following attribute:

```
// ReSharper disable once CheckNamespace
namespace Padded.Fody
{
    public sealed class PaddedAttribute : Attribute
    {
    }
}
```

Every non-abstract class or struct that is marked with this attribute will have a padding applied, that should remove one of the most common concurrency related problems: [the false sharing problem](http://mechanical-sympathy.blogspot.com/2011/07/false-sharing.html).
