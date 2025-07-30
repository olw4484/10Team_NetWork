using UnityEngine;

public class InventoryPresenter
{
    private readonly ILoginView view;
    private readonly SHI_Inventory inventory;

    public InventoryPresenter(ILoginView view, SHI_Inventory inventory)
    {
        this.view = view;
        this.inventory = inventory;
    }
}
