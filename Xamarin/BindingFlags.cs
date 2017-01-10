using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XamarinApp.LocalDb.Helpers
{
    /// <summary>
    /// Specifies flags that control binding and the way in which the search for 
    /// members and types is conducted by reflection.
    /// </summary>
    [Flags]
    public enum BindingFlags
    {
        Default = 0,
        CreateInstance = 1,
        DeclaredOnly = 2,
        ExactBinding = 4,
        FlattenHierarchy = 8,
        GetField = 16,
        GetProperty = 32,
        IgnoreCase = 64,
        IgnoreReturn = 128,
        Instance = 256,
        InvokeMethod = 512,
        NonPublic = 1024,
        OptionalParamBinding = 2048,
        Public = 4096,
        PutDispProperty = 8192,
        PutRefDispProperty = 16384,
        SetField = 32768,
        SetProperty = 65536,
        Static = 131072,
        SuppressChangeType = 262144
    }
}
