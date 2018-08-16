## `IWshShell`

```csharp
public interface IWshRuntimeLibrary.IWshShell

```

## `IWshShell2`

```csharp
public interface IWshRuntimeLibrary.IWshShell2
    : IWshShell

```

## `IWshShell3`

```csharp
public interface IWshRuntimeLibrary.IWshShell3
    : IWshShell2, IWshShell

```

Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `Object` | CreateShortcut(`String` PathLink) |  | 


## `IWshShortcut`

```csharp
public interface IWshRuntimeLibrary.IWshShortcut

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | Arguments |  | 
| `String` | Description |  | 
| `String` | FullName |  | 
| `String` | IconLocation |  | 
| `String` | TargetPath |  | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `void` | Save() |  | 


## `WshShell`

```csharp
public interface IWshRuntimeLibrary.WshShell
    : IWshShell3, IWshShell2, IWshShell

```

