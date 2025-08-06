using UnityEngine;

public class InventoryHUDPresenter
{
    private readonly InventoryHUDView view;
    private readonly InventoryHUDModel model;

    public InventoryHUDPresenter(InventoryHUDView view, InventoryHUDModel model)
    {
        this.view = view;
        this.model = model;

        // ����
        SubscribeEvents();

        // �ʱ�ȭ
        InitView();
    }

    private void SubscribeEvents()
    {
        model.Inventory.itemadd.AddListener(OnInventoryChanged);
        model.Inventory.itemremove.AddListener(OnInventoryChanged);
    }

    public void Dispose()
    {
        model.Inventory.itemadd.RemoveListener(OnInventoryChanged);
        model.Inventory.itemremove.RemoveListener(OnInventoryChanged);
    }

    private void InitView()
    {
        view.InitSlots(model);
        view.UpdateSlots(model.Inventory.items);
    }

    private void OnInventoryChanged()
    {
        view.UpdateSlots(model.Inventory.items);
    }
}