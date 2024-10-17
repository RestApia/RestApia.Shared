using Newtonsoft.Json.Linq;
namespace RestApia.Extensions.Import.Postman.Abstract;

internal record JDataModel(JObject Data, PostmanJsonType Type);
