using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

public class GameDataConverter : JsonConverter<GameData>
{
    public override void WriteJson(JsonWriter writer, GameData value, JsonSerializer serializer)
    {
        writer.WriteStartObject();

        foreach (var field in value!.GetType().GetFields())
        {
            writer.WritePropertyName(field.Name);
            serializer.Serialize(writer, field.GetValue(value));
        }

        writer.WriteEndObject();
    }

    public override GameData ReadJson(JsonReader reader, Type objectType, GameData existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject data = JObject.Load(reader);
        GameData gameData = new GameData();

        foreach (var item in data.Children())
        {
            if (item.Type == JTokenType.Property)
            {
                var property = (JProperty)item;

                if (property.Name == nameof(gameData.objectData))
                {
                    foreach (var child in property.Children().Children())
                    {
                        var myObjectProperty = (JProperty)child;

                        Type t = Type.GetType(myObjectProperty.Name);

                        if (t != null)
                        {
                            var jsonMethod = typeof(JsonConvert).GetMethods().First(x => x.Name == "DeserializeObject" && x.IsGenericMethod == true)!.MakeGenericMethod(new Type[] { t })!;
                            var gameDataMethod = gameData.GetType().GetMethod("SetData")!.MakeGenericMethod(new Type[] { t })!;

                            foreach (var prop in myObjectProperty.Children())
                            {
                                foreach (var c in prop.Children())
                                {
                                    var result = jsonMethod.Invoke(null, new object[] { c.First().ToString() })!;
                                    gameDataMethod.Invoke(gameData, new object[] { ((JProperty)c).Name, result });
                                }
                            }
                        }
                    }
                }
                else
                {
                    var gameDataField = gameData.GetType().GetField(property.Name)!;
                    if (item.Type == JTokenType.Property)
                    {
                        if (gameDataField.FieldType == typeof(string))
                        {
                            gameDataField.SetValue(gameData, property.Value.ToString());
                        }
                        else
                        {
                            var jsonMethod = typeof(JsonConvert).GetMethods().First(x => x.Name == "DeserializeObject" && x.IsGenericMethod == true)!.MakeGenericMethod(new Type[] { gameDataField.FieldType })!;
                            gameDataField.SetValue(gameData, jsonMethod.Invoke(null, new object[] { property.Value.ToString()! }));
                        }
                    }
                }
            }

        }

        return gameData;
    }

}
