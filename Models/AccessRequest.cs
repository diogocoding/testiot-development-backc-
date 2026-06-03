namespace AccessControlAPI.Models;

// Esta classe serve apenas para receber os dados do ESP32, não vai para o banco de dados.
public class AccessRequest
{
    public string RfidTag { get; set; } = string.Empty;
    public string DeviceToken { get; set; } = string.Empty;
}