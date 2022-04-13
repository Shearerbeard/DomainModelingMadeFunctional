// For more information see https://aka.ms/fsharp-console-apps

module Program

    type Undefined = exn
    type CustomerId = CustomerId of int
    type OrderId = OrderId of int
    type WidgetCode = WidgetCode of int
    type GizmoCode = GizmoCode of int
    type UnitQuantity = private UnitQuantity of int
    type KilogramQuantity = KilogramQuantity of decimal

    type ProductCode = 
        | Widget of WidgetCode
        | Gizmo of GizmoCode

    type OrderQuantity =
        | Unit of UnitQuantity
        | Kilogram of KilogramQuantity

    type CustomerInfo = Undefined
    type ShippingAddress = Undefined
    type BillingAddress = Undefined
    type OrderLine = Undefined
    type BillingAmount = Undefined

    type Order = {
        CustomerInfo : CustomerInfo
        ShippingAddress : ShippingAddress
        BillingAddress : BillingAddress
        OrderLines : OrderLine list
        AmountToBill : BillingAmount
    }

    module UnitQuantity =
        let create qty =
            if qty < 1 then
                Error "Unit Quantity Cannot Be Negative"
            else if qty > 1000 then
                Error "Unit Quantity Cannot be more than 1000"
            else
                Ok(UnitQuantity qty)

        let value (UnitQuantity qty) = qty

    let main =
        let customerId = CustomerId 42
        let (CustomerId innerValue) = customerId
        printfn "%i" innerValue