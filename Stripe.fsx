#load @".paket/load/FunStripe.fsx"
#load @".paket/load/dotenv.net.fsx"

open FunStripe.AsyncResultCE
open FunStripe.StripeRequest
open dotenv.net

DotEnv.Load()
let envars = DotEnv.Read()

let createCustomerAddress line1 line2 city state zip =
    async {
        return
            Customers.Create'AddressOptionalFieldsAddress.New(
                line1 = line1,
                line2 = line2,
                city = city,
                state = state,
                postalCode = zip
            )
    }


let createCustomer settings address =
    asyncResult {
        return!
            Customers.CreateOptions.New(name = "Name", address = Choice1Of2 address)
            |> Customers.Create settings
    }


let createCard () =
    PaymentMethods.Create'CardCardDetailsParams.New(
        cvc = "333",
        expMonth = 10,
        expYear = 2022,
        number = "4242424242424242"
    )

let createPaymentMethod settings card =
    asyncResult {
        return!
            PaymentMethods.CreateOptions.New(
                card = Choice1Of2 card,
                // card = Choice2Of2 card,
                type' = PaymentMethods.Create'Type.Card
            )
            |> PaymentMethods.Create settings
    }

let attachCustomer settings customerId paymentMethodId =
    asyncResult {
        return!
            PaymentMethodsAttach.AttachOptions.New(customer = customerId, paymentMethod = paymentMethodId)
            |> PaymentMethodsAttach.Attach settings
    }
