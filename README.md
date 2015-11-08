# DeferredJsonSerializer

Some content.

Usage:
```
var serializer = new DeferredJsonSerializer();
serializer.Serialize<SomeType>(someObject, Stream);
serializer.Deserialize<SomeType>(Stream); // currently works with objects only. Lists/Arrays not working.
```

Things are subject to change.
