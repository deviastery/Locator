namespace Shared.Options;

public class KafkaOptions
{
    public const string SECTION_NAME = "Kafka";
    public string BootstrapServers { get; init; } = default!;
}