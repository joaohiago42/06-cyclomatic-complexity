// ============================================================
// EXEMPLO RUIM: Switch gigante com 20+ cases
// ============================================================
// Inspirado no DebugController.Translate() real (~30 patterns, ~189 linhas)
//
// Cada case é um caminho — complexidade ciclomática = número de cases + 1

namespace CyclomaticComplexity.HighComplexity;

public class MessageTranslator
{
    // Complexidade: ~25
    public object Translate(int messageType, byte[] payload)
    {
        switch (messageType)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                return TranslateStateMessage(payload);
            case 5:
                return TranslateKeepAlive(payload);
            case 6:
                return TranslateLogin(payload);
            case 7:
                return TranslateLogout(payload);
            case 8:
            case 9:
                return TranslateAlert(payload);
            case 10:
                return TranslatePosition(payload);
            case 11:
                return TranslateSensorData(payload);
            case 12:
            case 13:
            case 14:
                return TranslateOperationalState(payload);
            case 15:
                return TranslateFuelLevel(payload);
            case 16:
                return TranslateSpeed(payload);
            case 17:
                return TranslateTemperature(payload);
            case 18:
            case 19:
                return TranslateHourmeter(payload);
            case 20:
                return TranslateDiagnostic(payload);
            // ... imagine mais 50 cases ...
            default:
                throw new NotSupportedException($"Message type {messageType} not supported");
        }
    }

    private object TranslateStateMessage(byte[] payload) => new { Type = "State" };
    private object TranslateKeepAlive(byte[] payload) => new { Type = "KeepAlive" };
    private object TranslateLogin(byte[] payload) => new { Type = "Login" };
    private object TranslateLogout(byte[] payload) => new { Type = "Logout" };
    private object TranslateAlert(byte[] payload) => new { Type = "Alert" };
    private object TranslatePosition(byte[] payload) => new { Type = "Position" };
    private object TranslateSensorData(byte[] payload) => new { Type = "Sensor" };
    private object TranslateOperationalState(byte[] payload) => new { Type = "OpState" };
    private object TranslateFuelLevel(byte[] payload) => new { Type = "Fuel" };
    private object TranslateSpeed(byte[] payload) => new { Type = "Speed" };
    private object TranslateTemperature(byte[] payload) => new { Type = "Temp" };
    private object TranslateHourmeter(byte[] payload) => new { Type = "Hourmeter" };
    private object TranslateDiagnostic(byte[] payload) => new { Type = "Diagnostic" };
}

// Problemas:
// - Cada novo tipo de mensagem = mais um case no switch
// - O switch só vai crescer — viola Open/Closed Principle
// - Para testar, precisa de 20+ testes só para este método
// - Se dois devs adicionam cases ao mesmo tempo = conflito de merge garantido
