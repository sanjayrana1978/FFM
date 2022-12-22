CREATE OR ALTER procedure [dbo].[CopyCustomerInformationClientDbToNopCommerceDb]
AS
--Declare varibales
DECLARE @count INT
DECLARE @Username varchar(max)
DECLARE @Email varchar(max)
DECLARE @Firstname varchar(max)
DECLARE @LastName varchar(max)
DECLARE @DisplayName varchar(max)
DECLARE @CreateUtcOn DateTime
DECLARE @CustomerGuid varchar(max)
DECLARE @CustomerId INT 
DECLARE @LastActivityDateUtc DateTime
DECLARE @Id INT = 1

--Get all users and store into temp table
SELECT ROW_NUMBER() OVER (ORDER BY CAST(GETDATE() AS TIMESTAMP)) As TempId ,* into #UsersData FROM [Pratima_FFM_0322].[foodequality_production_110322].[wp_wc_customer_lookup]

Select	@Email=email,
		@Username=username, 
		@Firstname = first_name,
		@Lastname = last_name
from #UsersData where TempId = 2; 

SELECT @CreateUtcOn = GETUTCDATE()
SELECT @CustomerGuid = NEWID()
SELECT @LastActivityDateUtc = GETUTCDATE()

--Fetch all row one by one
SELECT @count = Count(*) from #UsersData
WHILE @Id<= @count
BEGIN
	Select	
		@Email=email,
		@Username=username, 
		@Firstname = first_name,
		@Lastname = last_name
	from #UsersData where TempId = @Id;

	If NOT EXISTS(SELECT * FROM [dbo].[Customer] Where Email = @Email)
	BEGIN
		If @Email is not null
		BEGIN
			SELECT @CreateUtcOn = GETUTCDATE()
			SELECT @CustomerGuid = NEWID()
			SELECT @LastActivityDateUtc = GETUTCDATE()

			INSERT INTO [dbo].[Customer] 
			([Username],[Email],[Active],[CustomerGuid],[CreatedOnUtc],[RegisteredInStoreId],[IsTaxExempt],[AffiliateId],[VendorId],[HasShoppingCartItems],[RequireReLogin],[FailedLoginAttempts],[Deleted],[IsSystemAccount],[LastActivityDateUtc],[AdminComment]) 
			Values(@Username,@Email,1,@CustomerGuid,@CreateUtcOn,0,0,0,0,0,0,0,0,0,@LastActivityDateUtc,'1') 
			SET @CustomerId = @@IDENTITY

			INSERT INTO [dbo].[Customer_CustomerRole_Mapping] ([Customer_Id],[CustomerRole_Id]) VALUES (@CustomerId,3)

			INSERT [dbo].[CustomerPassword] 
			([CustomerId],[Password],[PasswordFormatId],[CreatedOnUtc])
			Values(@CustomerId,'1234567890',0,GETUTCDATE())

			IF @Firstname IS NOT NULL
			BEGIN
				INSERT INTO [dbo].[GenericAttribute]
				([KeyGroup],[Key],[Value],[EntityId],[StoreId],[CreatedOrUpdatedDateUTC]) 
				VALUES('Customer','FirstName',@Firstname,@CustomerId,0,@CreateUtcOn)
			END

			IF @Firstname IS NOT NULL
			BEGIN
				INSERT INTO [dbo].[GenericAttribute]
				([KeyGroup],[Key],[Value],[EntityId],[StoreId],[CreatedOrUpdatedDateUTC]) 
				VALUES('Customer','LastName',@Lastname,@CustomerId,0,@CreateUtcOn)
			END
		END
	END
	SET @Id = @Id + 1
END