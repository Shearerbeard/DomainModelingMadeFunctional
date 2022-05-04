module Payment =
    open FunStripe.StripeRequest

    let createSubscription settings =
        Subscriptions.CreateOptions.New(
            customer = "asdf",
            items = [Subscriptions.Create'Items.New(
                price = "asdf",
                quantity = 1
            )]
        )
        |> Subscriptions.Create settings

    let createSession settings  =
        let item = CheckoutSessions.Create'LineItems.New(
                price = "asdf",
                quantity = 1
        )
        CheckoutSessions.CreateOptions.New(
            mode = CheckoutSessions.Create'Mode.Subscription,
            customerEmail = "asdf",
            lineItems = [item],
            cancelUrl = "asdf",
            successUrl = "asdf"
        )
        |> CheckoutSessions.Create settings


module Reko =

    open System
    open FsToolkit.ErrorHandling

    (*
            TODO:
                Email + Text notification preference
                Notifcations + through web

            TODO:
                Class Signup Module
        *)
    (*
        Simple Types
        *)
    type Undefined = exn
    type ID<'T> = ID of System.Guid
    type UnvalidatedEmail = UnvalidatedEmail of string
    type UnverifiedEmail = private UnverifiedEmail of string
    type VerifiedEmail = private VerifiedEmail of string
    type ZipCode = private ZipCode of string
    type UserId = UserId of ID<UserId>
    type RingId = RingId of ID<RingId>
    type PostId = PostId of ID<PostId>
    type CustomerId = CustomerId of ID<UserId>
    type AdminId = AdminId of ID<AdminId>
    type VendorId = VendorId of string
    type Password = private Password of string

    type ActorId =
        | Admin of AdminId
        | Customer of CustomerId
        | Vendor of VendorId

    (*
            Constrained Simple Types
        *)
    module ConstrainedType =
        let newId<'T> () : ID<'T> = ID(System.Guid.NewGuid())


    module UnverifiedEmail =
        let value (UnverifiedEmail str) = str

        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Email: Must not be null or empty"
            elif System.Text.RegularExpressions.Regex.IsMatch(str, ".+@.+") then
                Ok(UnverifiedEmail str)
            else
                Error "Invalid Email Address"

    module VerifiedEmail =
        let value (VerifiedEmail str) = str

    module ZipCode =
        let value (ZipCode str) = str

        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Zip Code Must Not Be Empty"
            elif System.Text.RegularExpressions.Regex.IsMatch(str, "\d{5}") then
                Ok(ZipCode str)
            else
                Error "Invalid Zip Code"

    module Password =
        let value (Password str) = str

        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Email: Must not be null or empty"
            else
                Ok(Password str)

    (*
            Compound Types
        *)
    type AddressUSA =
        { AddressLine1: string
          AddressLine2: string
          City: string
          State: string
          ZipCode: ZipCode }

    type Address = AddressUSA

    type Email =
        | VerifiedEmail
        | UnverifiedEmail

    type PersonalName = { FirstName: string; LastName: string }


    (*
            User Types
        *)
    type User =
        { Id: UserId
          Name: PersonalName
          Email: Email
          Password: Password }

    (*
            Ring Types
        *)
    type RingName = RingName of string
    type RingSchedule = Undefined

    type UnvalidatedRing =
        { Name: RingName
          Address: Address
          Schedule: RingSchedule }

    type ValidatedRing =
        { Id: RingId
          Name: RingName
          Address: Address
          Schedule: RingSchedule }

    type ArchivedRing =
        { Id: RingId
          Name: RingName
          Address: Address
          Schedule: RingSchedule }

    type Ring =
        | Unvalidated of UnvalidatedRing
        | Valid of ValidatedRing
        | Archived of ArchivedRing

    type VendorName = VendorName of string
    type VendorLogo = Undefined
    type VendorRing = VendorRing of RingId

    type Vendor =
        { Id: VendorId
          Name: VendorName
          Email: Email
          Logo: VendorLogo
          User: UserId
          VendorRings: VendorRing list }

    (*
            Customer Types
        *)
    type CustomerName = CustomerName of string
    type CustomerRing = CustomerRing of RingId

    type Customer =
        { Id: CustomerId
          user: UserId
          Name: CustomerName
          Rings: RingId list }

    (*
            Admin Types
        *)
    type AdminName = AdminName of string

    type Admin = { Id: AdminId; UserId: UserId }

    (*
            Post Types
        *)
    type PostAuthor =
        | Vendor of VendorId
        | Admin of AdminId
        | Customer of CustomerId

    type PostText = PostText of string
    type PostParent = PostParent of PostId option

    type RootPost =
        { Id: PostId
          Author: PostAuthor
          Text: PostText }

    type ReplyPost =
        { Id: PostId
          Author: PostAuthor
          Text: PostText
          Parent: PostParent }

    type ReplyPostWithMention =
        { Id: PostId
          Author: PostAuthor
          Text: PostText
          Parent: PostParent
          Mention: PostAuthor }

    type Post =
        | Root of RootPost
        | Reply of ReplyPost
        | Mention of ReplyPostWithMention

    (*
        Util
    *)
    let predicatePassthrough f x err =
        if f x then Ok(x) else Error(err)

    let toResultPassthrough f x err =
        match f x with
            | Ok(y) -> Ok(x)
            | Error(_) -> Error(err)

    let toSomePassthrough f x err =
        match f x with
            | Some(y) -> Ok(y)
            | None -> Error(err)

    (*
            Implementation
        *)

    type ValidateEmail = UnvalidatedEmail -> Result<UnverifiedEmail, EmailValidationError>

    and EmailValidationError =
        | EmptyEmail
        | InvalidEmail

    type VerifyEmail = UnverifiedEmail -> VerifiedEmail

    (*
            Command
        *)
    type CreateRing =
        { UnvalidatedRing: UnvalidatedRing
          ActorId: ActorId }

    type UpdateRing =
        { Ring: ValidatedRing
          Name: RingName option
          Address: Address option
          Schedule: RingSchedule option
          ActorId: ActorId }

    type RemoveRing = { RingId: RingId; ActorId: ActorId }

    type Command<'data> =
        { Data: 'data
          TimeStamp: DateTime
          UserId: UserId
          ActorId: ActorId }

    type CreateRingCommand = Command<CreateRing>
    type UpdateRingCommand = Command<UpdateRing>
    type RemoveRingCommand = Command<RemoveRing>

    (*
            Events
        *)
    type RingCreatedEvent =
        { AdminId: AdminId
          Ring: ValidatedRing }

    type RingUpdatedEvent = { AdminId: AdminId; Ring: UpdateRing }

    type RingRemovedEvent = { AdminId: AdminId; RingId: RingId }

    (*
            Workflow Steps
    *)
    // ValidateRing

    type RingNameTakenError =
        | RingNameTakenError
        | ApiError

    type HasRingAdminError =
        | HasRingAdminError
        | ApiError

    type HasRingExistsError =
        | HasringExistsError
        | ApiError

    type CreateRingId = unit -> ID<RingId>
    type HasRingAdmin = ActorId -> Async<Result<bool, exn>>
    type RingExists = RingId -> Async<bool>
    type RingNameTaken = RingName -> Async<Result<bool, exn>>
    type HasValidRingSchedule = RingSchedule -> RingSchedule option

    type ValidateRingCreate =
        HasRingAdmin // dependency
            -> RingNameTaken // dependency
            -> HasValidRingSchedule // dependency
            -> CreateRingId
            -> CreateRing // input
            -> Async<Result<ValidatedRing, ValidateRingError>>

    and ValidateRingError =
        | AlreadyExists
        | UserNotAdmin
        | NameTaken of RingNameTakenError
        | InvalidSchedule

    type ValidateRingUpdate =
        HasRingAdmin // dependency
            -> RingExists // dependency
            -> RingNameTaken // dependency
            -> HasValidRingSchedule // dependency
            -> UpdateRing // input
            -> Async<Result<ValidatedRing, ValidateRingUpdateError>>

    and ValidateRingUpdateError =
        | AlreadyExists
        | UserNotAdmin
        | NameTaken of RingNameTakenError
        | InvalidSchedule

    type RingIsRemoved = Undefined

    type ValidateRingRemove =
        RingExists // dependency
            -> RingIsRemoved // dependency
            -> HasRingAdmin // dependency
            -> RingNameTaken // dependency
            -> HasValidRingSchedule // dependency
            -> RemoveRing // input
            -> Result<ValidatedRing, ValidateRingUpdateError>

    and ValidateRingRemoveError =
        | DoesNotExist
        | AlreadyRemoved
        | UserNotAdmin
        | NameTaken
        | InvalidSchedule

    (*
            Workflow Impl
    *)
    type AddRingWorkflow = CreateRingCommand -> Result<RingCreatedEvent, AddRingWorkflowError>
    and AddRingWorkflowError = Validation of ValidateRingError

    type UpdateRingWorkflow = UpdateRingCommand -> Result<RingUpdatedEvent, UpdateRingWorkflowError>
    and UpdateRingWorkflowError = Validation of ValidateRingUpdateError

    type RemoveRingWorkflow = RemoveRingCommand -> Result<RingRemovedEvent, RemoveRingWorkflowError>
    and RemoveRingWorkflowError = Validation of ValidateRingRemoveError

    let validateRingCreate: ValidateRingCreate =
        fun hasRingAdmin ringNameTaken hasValidRingSchedule createRingId input ->
            asyncResult {
                let inputName = input.UnvalidatedRing.Name

                // TODO - consolidate actorId check
                do!
                    input.ActorId
                    |> hasRingAdmin
                    |> AsyncResult.foldResult
                        (fun x ->
                            if x then
                                Ok(())
                            else
                                Error(ValidateRingError.UserNotAdmin))
                        (fun _ -> Error(ValidateRingError.UserNotAdmin))

                // TODO - wrapper function to lift value out of predicate check
                let! name =
                    inputName
                    |> ringNameTaken
                    |> AsyncResult.foldResult
                        (fun x ->
                            if x then
                                Ok(inputName)
                            else
                                Error(ValidateRingError.NameTaken RingNameTakenError.RingNameTakenError))
                        (fun e -> Error(ValidateRingError.NameTaken RingNameTakenError.ApiError))

                // TODO - wrapper function to lift value out of predicate check
                let! schedule =
                    input.UnvalidatedRing.Schedule
                    |> hasValidRingSchedule
                    |> Result.requireSome ValidateRingError.InvalidSchedule

                let id = createRingId ()

                let validatedRing: ValidatedRing =
                    { Id = RingId id
                      Name = name
                      Address = input.UnvalidatedRing.Address
                      Schedule = schedule }

                return validatedRing
            }

    let validateRingUpdate: ValidateRingUpdate =
        fun hasRingAdmin ringExists ringNameTaken hasValidRingSchedule input ->
            asyncResult {
                let ring = input.Ring

                // TODO - consolidate actorId check
                do!
                    input.ActorId
                    |> hasRingAdmin
                    |> AsyncResult.foldResult
                        (fun x ->
                            if x then
                                Ok(())
                            else
                                Error(ValidateRingUpdateError.UserNotAdmin))
                        (fun _ -> Error(ValidateRingUpdateError.UserNotAdmin))

                do!
                    input.Ring.Id
                    |> ringExists
                    |> AsyncResult.requireTrue ValidateRingUpdateError.AlreadyExists


                // TODO - wrapper function to lift value out of predicate check
                let! name =
                    input.Name
                    |> Option.defaultValue ring.Name
                    |> ringNameTaken
                    |> AsyncResult.foldResult
                        (fun x ->
                            if x then
                                Ok(Option.defaultValue ring.Name input.Name)
                            else
                                Error(ValidateRingUpdateError.NameTaken RingNameTakenError.RingNameTakenError))
                        (fun _ -> Error(ValidateRingUpdateError.NameTaken RingNameTakenError.ApiError))

                // TODO - wrapper function to lift value out of predicate check
                let! schedule =
                    input.Schedule
                    |> Option.defaultValue ring.Schedule
                    |> hasValidRingSchedule
                    |> Result.requireSome ValidateRingUpdateError.InvalidSchedule

                let rv =
                    { ring with
                        Name = name
                        Schedule = schedule }

                return rv
            }
