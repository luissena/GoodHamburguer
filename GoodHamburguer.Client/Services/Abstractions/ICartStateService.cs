namespace GoodHamburguer.Client.Services.Abstractions;

public interface ICartStateService
{
    Guid? GetCurrentCartId();
    void SetCurrentCartId(Guid cartId);
    void ClearCart();
}


