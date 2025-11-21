namespace GoodHamburguer.Client.Services;

public class CartStateService : ICartStateService
{
    private Guid? _currentCartId;

    public Guid? GetCurrentCartId()
    {
        return _currentCartId;
    }

    public void SetCurrentCartId(Guid cartId)
    {
        _currentCartId = cartId;
    }

    public void ClearCart()
    {
        _currentCartId = null;
    }
}

