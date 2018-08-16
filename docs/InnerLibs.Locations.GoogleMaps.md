## `Map`

```csharp
public class InnerLibs.Locations.GoogleMaps.Map

```

Properties

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | APIKey | Chave de API do Google Maps | 
| `Uri` | ApiUrl | URL da a API do Google Maps | 
| `String` | MAP_ID | ID do Mapa | 
| `List<Marker>` | Markers | Marcadores do Mapa | 
| `String` | ScriptTag | Tag Script contendo a URL da Api no SRC (Adicione no Header ou Body) | 


Methods

| Type | Name | Summary | 
| --- | --- | --- | 
| `String` | MakeMap(`String` MAP_ID, `Int32` Zoom = 11, `String` Height = 200px, `Location` Center = null) | Constroi uma div com o Mapa | 


