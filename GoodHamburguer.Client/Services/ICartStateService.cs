namespace GoodHamburguer.Client.Services;

public interface ICartStateService
{
    Guid? GetCurrentCartId();
    void SetCurrentCartId(Guid cartId);
    void ClearCart();
}

