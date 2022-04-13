module Reko
    (*
        General Types
    *)
    type Undefined = exn

    (*
        Ring Types
    *)
    type RingId = RingId of string
    type RingName = RingName of string
    type RingAddress = Undefined

    type Ring = {
        RingId : RingId
        RingName : RingName
        RingAddress : RingAddress
    }

    (*
        Vendor Types
    *)
    type VendorId = VendorId of string
    type VendorName = Undefined
    type VendorEmail = VendorEmail of string
    type VendorLogo = Undefined
    type VendorRing = VendorRing of RingId

    type Vendor = {
        VendorId: VendorId
        VendorName : VendorName
        VendorEmail : VendorEmail
        VendorLogo : VendorLogo
        VendorRings : VendorRing list
    }

    (*
        Customer Types
    *)
    type CustomerId = CustomerId of string
    type CustomerName = Undefined
    type CustomerEmail = CustomerEmail of string
    type CustomerRing = CustomerRing of RingId


    type Customer = {
        CustomerId :  CustomerId
        CustomerName : CustomerName
        CustomerRings : RingId list
    }

    (*
        Admin Types
    *)
    type AdminId = AdminId of string
    type AdminName = Undefined
    type AdminEmail = AdminEmail of string

    type Admin = {
        AdminId : AdminId
        AdminName : AdminName
        AdminEmail : AdminEmail
    }

    (*
        Post Types
    *)
    type PostId = PostId of string
    type PostAuthor =
        | Vendor of VendorId
        | Admin of AdminId
        | Customer of CustomerId
    type PostText = PostText of string
    type PostParent = PostParent of PostId option
    type RootPost = {
        PostId : PostId
        PostAuthor : PostAuthor
        PostText : PostText
    }

    type ReplyPost = {
        PostId : PostId
        PostAuthor : PostAuthor
        PostText : PostText
        PostParent : PostParent
    }

    type ReplyPostWithMention = {
        PostId : PostId
        PostAuthor : PostAuthor
        PostText : PostText
        PostParent : PostParent
        PostMention : PostAuthor
    }

    type Post =
        | Root of RootPost
        | Reply of ReplyPost
        | Mention of ReplyPostWithMention