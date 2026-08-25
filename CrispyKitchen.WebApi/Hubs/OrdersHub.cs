using System.Security.Claims;
using CrispyKitchen.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CrispyKitchen.WebApi.Hubs;

[Authorize]
public class OrdersHub : Hub
{
    private readonly IUnitOfWork _unitOfWork;

    public OrdersHub(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task SubscribeToOrder(Guid orderId)
    {
        var userIdValue = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
            throw new HubException("Unable to identify the current user.");

        var order = await _unitOfWork.Orders.GetByIdAsync(orderId)
            ?? throw new HubException("Order not found.");

        var isStaff = Context.User?.IsInRole("Admin") == true || Context.User?.IsInRole("KitchenStaff") == true;
        if (!isStaff && order.CustomerId != userId)
            throw new HubException("You are not allowed to subscribe to this order.");

        await Groups.AddToGroupAsync(Context.ConnectionId, OrderGroup(orderId));
    }

    public async Task SubscribeToKitchen()
    {
        var isStaff = Context.User?.IsInRole("Admin") == true || Context.User?.IsInRole("KitchenStaff") == true;
        if (!isStaff)
            throw new HubException("Kitchen access is required.");

        await Groups.AddToGroupAsync(Context.ConnectionId, KitchenGroup);
    }

    public static string OrderGroup(Guid orderId) => $"order-{orderId}";
    public const string KitchenGroup = "kitchen";
}
