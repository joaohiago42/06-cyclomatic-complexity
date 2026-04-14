// ============================================================
// EXEMPLO BOM: Dictionary pattern — elimina switch gigante
// ============================================================
// Inspirado no EquipmentMessageProcessor.cs real que usa
// Dictionary<int, Func<...>> para rotear 71 tipos de mensagem.
//
// Complexidade do método Translate: 2 (if do TryGetValue + base)
// Os 20+ cases sumiram — agora são entradas num dicionário.

namespace CyclomaticComplexity.LowComplexity;

public interface IMessageHandler
{
    object Translate(byte[] payload);
}

public class MessageTranslator
{
    private readonly Dictionary<int, IMessageHandler> _handlers;

    public MessageTranslator(IEnumerable<IMessageHandler> handlers)
    {
        // Registro: cada handler sabe quais tipos ele trata
        _handlers = new Dictionary<int, IMessageHandler>
        {
            [1] = new StateMessageHandler(),
            [2] = new StateMessageHandler(),
            [3] = new StateMessageHandler(),
            [5] = new KeepAliveHandler(),
            [6] = new LoginHandler(),
            [7] = new LogoutHandler(),
            [10] = new PositionHandler(),
            [15] = new FuelLevelHandler(),
            // ... quantos forem necessários
        };
    }

    // Complexidade: 2 (apenas o if do TryGetValue)
    public object Translate(int messageType, byte[] payload)
    {
        if (!_handlers.TryGetValue(messageType, out var handler))
            throw new NotSupportedException($"Message type {messageType} not supported");

        return handler.Translate(payload);
    }
}

// Cada handler é uma classe simples com complexidade 1
public class StateMessageHandler : IMessageHandler
{
    public object Translate(byte[] payload) => new { Type = "State" };
}

public class KeepAliveHandler : IMessageHandler
{
    public object Translate(byte[] payload) => new { Type = "KeepAlive" };
}

public class LoginHandler : IMessageHandler
{
    public object Translate(byte[] payload) => new { Type = "Login" };
}

public class LogoutHandler : IMessageHandler
{
    public object Translate(byte[] payload) => new { Type = "Logout" };
}

public class PositionHandler : IMessageHandler
{
    public object Translate(byte[] payload) => new { Type = "Position" };
}

public class FuelLevelHandler : IMessageHandler
{
    public object Translate(byte[] payload) => new { Type = "Fuel" };
}

// Vantagens:
// - Translate() tem complexidade 2, independente de quantos tipos existam
// - Novo tipo de mensagem? Crie um handler e adicione ao dicionário
// - Cada handler pode ser testado isoladamente
// - Sem conflito de merge: cada handler é um arquivo separado
// - O dicionário pode ser montado via DI (registrando handlers automaticamente)
//
// Este é o mesmo padrão do EquipmentMessageProcessor.cs real,
// que usa Dictionary<int, Func<long, EquipmentMessageEnvelope, Task>>
// com 71 entradas — e o método de dispatch tem complexidade 2.
