# DBJ*InterSharp
**DBJ Interop Mechanism**

Extensive low level usage of C# COM+

TODO: Replace XML with JSON.

For testing to work each COM+ object you build has to iherit from the 
dbj.integration.Ipoint 
an interface defined in dbjipoint.dll assembly 
For non .NET code dbjipoint.tlb, type library is available

Easiest is to use the Co-Class available form the same assembly.
Since it is in the same dll as a required interface 
finding and instantiating this class will always work.
Of course AFTER dbjipoint.dll is properly registered for COM interop!

This project requires the knowledge on how to build an C# COM+ object.
I will add project that has one.

DBJ 2018JAN
