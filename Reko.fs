module Reko =
    open System
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
    type RingId = RingId of ID<UserId>
    type PostId = PostId of ID<UserId>
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
        let newId<'T> (): ID<'T> = ID(System.Guid.NewGuid())


    module UnverifiedEmail =
        let value (UnverifiedEmail str) = str 

        let create str =
            if String.IsNullOrEmpty(str) then
                Error "Email: Must not be null or empty"
            elif System.Text.RegularExpressions.Regex.IsMatch(str, ".+@.+") then
                Ok (UnverifiedEmail str)
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
                Ok (ZipCode str)
            else
                Error "Invalid Zip Code"

    module Password =
        let value (Password str) = str

        let create str = 
            if String.IsNullOrEmpty(str) then
                Error "Email: Must not be null or empty"
            else
                Ok (Password str)

    (*
        Compound Types
    *)
    type AddressUSA = {
        AddressLine1 : string
        AddressLine2 : string
        City : string
        State : string
        ZipCode : ZipCode
    }

    type Address = AddressUSA

    type Email =
        | VerifiedEmail
        | UnverifiedEmail

    type PersonalName = {
        FirstName : string
        LastName : string
    }
    

    (*
        User Types
    *)
    type User = {
        Id : UserId
        Name : PersonalName
        Email : Email
        Password : Password
    }

    (*
        Ring Types
    *)
    type RingName = RingName of string
    type RingSchedule = Undefined

    type UnvalidatedRing = {
        Name : RingName
        Address : Address
        Schedule : RingSchedule
    }

    type ValidatedRing = {
        Id : RingId
        Name : RingName
        Address : Address
        Schedule : RingSchedule
    }

    type ArchivedRing = {
        Id : RingId
        Name : RingName
        Address : Address
        Schedule : RingSchedule
    }

    type Ring =
        | Unvalidated of UnvalidatedRing
        | Valid of ValidatedRing
        | Archived of ArchivedRing

    (*
        Vendor Types
        Be member and vendor
    *)
    type VendorName = VendorName of string
    type VendorLogo = Undefined
    type VendorRing = VendorRing of RingId

    (*
        Email + Text notification preference
        Notifcations + through web
    *)
    type Vendor = {
        Id: VendorId
        Name : VendorName
        Email : Email
        Logo : VendorLogo
        User : UserId
        VendorRings : VendorRing list
    }

    (*
        Customer Types
    *)
    type CustomerName = CustomerName of string
    type CustomerRing = CustomerRing of RingId


    type Customer = {
        Id :  CustomerId
        user: UserId
        Name : CustomerName
        Rings : RingId list
    }

    (*
        Class Signup
    *)

    (*
        Admin Types
    *)
    type AdminName = AdminName of string

    type Admin = {
        Id : AdminId
        UserId : UserId
    }

    (*
        Post Types
    *)
    type PostAuthor =
        | Vendor of VendorId
        | Admin of AdminId
        | Customer of CustomerId
    type PostText = PostText of string
    type PostParent = PostParent of PostId option
    type RootPost = {
        Id : PostId
        Author : PostAuthor
        Text : PostText
    }

    type ReplyPost = {
        Id : PostId
        Author : PostAuthor
        Text : PostText
        Parent : PostParent
    }

    type ReplyPostWithMention = {
        Id : PostId
        Author : PostAuthor
        Text : PostText
        Parent : PostParent
        Mention : PostAuthor
    }

    type Post =
        | Root of RootPost
        | Reply of ReplyPost
        | Mention of ReplyPostWithMention

    (*
        Implementation
    *)

    type ValidateEmail = UnvalidatedEmail -> Result<UnverifiedEmail, EmailValidationError>
    and
        EmailValidationError =
            | EmptyEmail
            | InvalidEmail

    type VerifyEmail = UnverifiedEmail -> VerifiedEmail

    (*
        Workflow Components
    *)

    // ValidateRing
    type RingExists = Undefined
    type RingNameTaken = Undefined
    type HasRingAdmin = Undefined
    type HasValidRingSchedule = Undefined

    type RingCreate = UnvalidatedRing

    type ValidateRingCreate =
        RingExists // dependency
            -> HasRingAdmin // dependency
            -> RingNameTaken // dependency
            -> HasValidRingSchedule // dependency
            -> RingCreate // input
            -> Result<ValidatedRing, ValidateRingError>
    and
        ValidateRingError =
            | AlreadyExists
            | UserNotAdmin
            | NameTaken
            | InvalidSchedule

    type RingUpdate = {
        Ring : ValidatedRing
        Name : RingName option
        Address : Address option
        Schedule : RingSchedule
    }
    

    type ValidateRingUpdate =
        RingExists // dependency 
            -> HasRingAdmin // dependency 
            -> RingNameTaken // dependency 
            -> HasValidRingSchedule // dependency 
            -> RingUpdate // input
            -> Result<ValidatedRing, ValidateRingUpdateError> 
    and
        ValidateRingUpdateError =
            | DoesNotExist
            | UserNotAdmin
            | NameTaken
            | InvalidSchedule

    type RingRemove = {
        RingId : RingId
        ActorId : ActorId
    }

    type RingIsRemoved = Undefined

    type ValidateRingRemove =
        RingExists // dependency 
            -> RingIsRemoved
            -> HasRingAdmin // dependency 
            -> RingNameTaken // dependency 
            -> HasValidRingSchedule // dependency 
            -> RingRemove // input
            -> Result<ValidatedRing, ValidateRingUpdateError> 
    and
        ValidateRingRemoveError =
            | DoesNotExist
            | AlreadyRemoved
            | UserNotAdmin
            | NameTaken
            | InvalidSchedule

    // CreateRingId
    type CreateRingId = unit -> ID<RingId>

    (*
        Events
    *)
    type RingAddedEvent = {
        AdminId : AdminId
        Ring : ValidatedRing
    }

    type RingUpdatedEvent = {
        AdminId : AdminId
        Ring : RingUpdate
    }

    type RingRemovedEvent = {
        AdminId : AdminId
        RingId : RingId
    }

    (*
        Command
    *)
    type Command<'data> = {
        Data : 'data
        TimeStamp : DateTime
        UserId : UserId
        ActorId : ActorId
    }

    type AddRingCommand = Command<RingCreate>
    type UpdateRingCommand = Command<RingUpdate>
    type RemoveRingCommand = Command<RingRemove>

    (*
        Workflow Impl
    *)
    type AddRingWorkflow = AddRingCommand -> Result<RingAddedEvent, AddRingWorkflowError>
    and
        AddRingWorkflowError =
            | Validation of ValidateRingError

    type UpdateRingWorkflow = UpdateRingCommand -> Result<RingUpdatedEvent, UpdateRingWorkflowError>
    and
        UpdateRingWorkflowError =
            | Validation of ValidateRingUpdateError

    type RemoveRingWorkflow = RemoveRingCommand -> Result<RingRemovedEvent, RemoveRingWorkflowError>
    and
        RemoveRingWorkflowError =
            | Validation of ValidateRingRemoveError 

