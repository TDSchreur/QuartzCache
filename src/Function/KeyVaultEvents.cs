// using Azure.Messaging;
// using Azure.Messaging.EventGrid;
// using Azure.Messaging.EventGrid.SystemEvents;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Extensions.Logging;
//
// namespace TDS.QuartzCache.Function;
//
// public class KeyVaultEvents(ILogger<KeyVaultEvents> logger)
// {
//     [Function(nameof(KeyVaultEvents))]
//     public void Run([EventGridTrigger] CloudEvent cloudEvent)
//     {
//         logger.LogInformation("Event type: {Type}, Event subject: {Subject}", cloudEvent.Type, cloudEvent.Subject);
//
//         switch (cloudEvent.Type)
//         {
//             case SystemEventNames.KeyVaultSecretNewVersionCreated:
//                 var eventData = cloudEvent.Data.ToObjectFromJson<KeyVaultSecretNewVersionCreatedEventData>();
//                 logger.LogInformation("Key vault secret new version created: {VaultName}, {SecretName}, {Version}", eventData.VaultName, eventData.ObjectName, eventData.Version);
//                 break;
//         }
//
//         try
//         {
//             var cloudEventAsJson = System.Text.Json.JsonSerializer.Serialize(cloudEvent);
//             logger.LogInformation("Cloud event as json: {CloudEvent}", cloudEventAsJson);
//         }
//         catch (Exception e)
//         {
//             logger.LogError(e, "An exception occurred while processing cloud event");
//         }
//
//         try
//         {
//             var cloudEventData = System.Text.Json.JsonSerializer.Serialize(cloudEvent.Data);
//             logger.LogInformation("Cloud event data: {CloudEventData}", cloudEventData);
//         }
//         catch (Exception e)
//         {
//             logger.LogError(e, "An exception occurred while processing cloud event date");
//         }
//     }
// }
