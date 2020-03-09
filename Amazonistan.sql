Create database amazonistan;
go
drop database amazonistan
go
use Amazonistan
go


Create table UserInfo
(
	ID int,
	Name varchar(50),
	Email varchar(50) unique,
	[Password] varchar(50) not null,
	Balance int default 0,				--User can have virtual wallet on website.
	[Address] varchar(100),				--Null at sign-up. Shipping address is filled when user proceeds to checkout page.
	PhoneNumber varchar(50) unique,		--Null at sign-up. Can be used later for 2-step verification.

	Primary key(ID),
	
)

Insert into UserInfo values (1,'Borgar','borgar@gmail.com','123',0,NULL,'123')
Insert into UserInfo values (2,'Basim','basim@gmail.com','key',0,NULL,'0349-668-7587')
Insert into UserInfo values (3,'Haider','haider@gmail.com','yo',0,NULL,'0320-696-9420')
Insert into UserInfo values (5,'Admin','admin@gmail.com','admin',10000,NULL,'0320-111-1111')

Select * from UserInfo

Update UserInfo
Set UserInfo.Balance = 5000000
Where UserInfo.Email = 'haider@gmail.com'

go
Create table Category
(
	Name varchar(50),

	primary key(Name) --No need for "ID" , here Category names are also unique.
)


Insert into Category values ('Electronics')
Insert into Category values ('CPUs')
Insert into Category values ('Clothes'),('Food Items')

go
Create table Product  
(
	ID int,
	Name varchar(50),
	Category varchar(50),	--Only giving category ID here, and referencing it to Category table.
	Price int,

	Primary key(ID),
	Foreign key(Category) References Category(Name)
)


Insert into Product values (1,'Pixel 4','Electronics',100000)
Insert into Product values (2,'Ryzen 7 3700','CPUs',50000)
Insert into Product values (3,'Lolo shirt','Clothes',1000)
Insert into Product values (4,'Teey Shirt','Clothes',2000)
Insert into Product values (5,'Dairy milk', 'Food Items', 100)
Insert into Product values (6,'Happy Burger', 'Food Items', 130)
go

Create table ProductQuantity
(
	productID int,
	Quantitiy int

	Foreign key(productID) References Product(ID)
)


Insert into ProductQuantity values (1,10)
Insert into ProductQuantity values (2,10)
Insert into ProductQuantity values (3,10)
Insert into ProductQuantity values (4,10)
Insert into ProductQuantity values (5,10)
Insert into ProductQuantity values (6,10)

go
create table Orders
(
	ID int, 
	userID int,
	productID int not null, 
	orderDate date not null, 
	deliveryDate date not null, --expected delivery date (between 20-60 days)

	primary key(ID),
	foreign key (userID) references userInfo(ID),
	foreign key (productID) references Product(ID),

	Check(userID != 1)

)

Insert into orders values (1,2,1,'2019-1-1','2019-2-1')


go
Create table OrderDetails
(
	orderID int,
	productID int,
	quantity int,

	Foreign key(orderID) References Orders(ID),
	Foreign key(productID) References Product(ID)
)

go
Create table Reviews
(
	ID int,
	userID int,
	Rating int,
	productID int,
	[Description] varchar(250),
	
	Primary key(ID),
	Foreign key(userID) References userInfo(ID),

	Check(Rating >= 0 and Rating <= 10)
	
)
Select * from Product

Insert into Reviews values (1,2,9,1,'Good software, bad hardware')
Insert into Reviews values (2,1,7,1,'Bad design')
Insert into Reviews values (3,1,2,5,'Expired'),(4,1,10,6,'Best BorgEr in Lahre')

go
create table ShoppingCart
(
	ID int,
	productID int,
	quantity int,
	price int,
	totalCost int

	Primary key(ID),
	Foreign key(productID) References Product(ID),
)
go

----------Client Procedures----------
-----1	Login Page-----
Create Procedure [Login]
(
	@Email varchar(50), 
	@Password varchar(16),
	@returnCheck int OUT
)
As
Begin
	If Exists
	(
		Select * 
		From UserInfo
		Where UserInfo.Email = @Email and UserInfo.[Password] = @Password
	)
	Begin
		Set @returnCheck = 1
		Print 'Login Succesful'
	End
	
	Else
	Begin
		Set @returnCheck = 0
		Print 'Login Failed (Combination does not match)'
	End
End
go

Declare @checkValue int
Exec [Login] 'haider@gmail.com', 'yo', @checkValue
go

-----2	Signup Page-----
Create Procedure Signup
(
	@userName varchar(50), 
	@userEmail varchar(50), 
	@userPassword varchar(16), 
	@userNumber varchar(50), 
	@userAddress varchar(50),
	@returncheck int OUT
)
As
Begin
	If Exists(Select * From UserInfo Where UserInfo.Email = @userEmail)
	Begin
		Set @returncheck = 0
		Print 'Signup Failed. Error 69: Email Already in Use.'
	End
	
	Else
	Begin
		Declare @ID int
		Select @ID = count(*) From UserInfo
		Set @ID = @ID + 1
		
		Insert Into UserInfo values (@ID, @userName, @userEmail, @userPassword,0, @userAddress, @userNumber)
		
		Set @returncheck = 1
		print 'SignUp successful of ' + @userName
	End
End
go

Declare @returnCheck int
Exec Signup 'omer', 'omer@gmail.com', 'omer123', '0320-0320','Shadman', @returncheck
go

select * from UserInfo
go

-----3	Buying Item From Store-----
go
Create procedure buyingItemWithQuantity
(
	@currentUserEmail varchar (50),
	@itemID int,
	@quantity int,
	@returnCheck int output
)
As 
Begin
	If (@quantity < (Select Quantitiy From ProductQuantity Where productID = @itemID))
	Begin
		Declare @id int
		Select @id = count(ID) + 1
		From orders

		declare @amountToBeDeducted int
		Select @amountToBeDeducted = p.Price
		From product as p
		Where p.ID = @itemID
		
		Set @amountToBeDeducted = @amountToBeDeducted * @quantity

		Update UserInfo
		Set Balance = Balance - @amountToBeDeducted
		Where UserInfo.Email = @currentUserEmail

		if
		(
			Select Balance
			from UserInfo
			Where Email = @currentUserEmail
		) > -1
		Begin
			Set @returnCheck = 1
			
			Update ProductQuantity
			Set Quantitiy = Quantitiy - @quantity
			Where productID = @itemID

			declare @userID int
			Select @userID = ID 
			From UserInfo
			Where Email = @currentUserEmail

			insert into orders values (@id, @userID, @itemID, getdate(), getdate() + 7)
			insert into OrderDetails values (@id, @itemID, @quantity)

		End
		
		Else
		begin
			Set @returnCheck = 0
			Update UserInfo
			Set Balance = Balance + @amountToBeDeducted
			Where Email = @currentUserEmail
	
			print 'Current balance is insufficient!'
		end

	End
	Else
		set @returnCheck = 2
	
End

declare @checkValue int
execute buyingItemWitbuyingItemWithQuantity'haider@gmail.com',2,2,@returnCheck = @checkValue out
select @checkValue
go


--Without Quantity--
go
Create procedure buyingItem
(
	@currentUserEmail varchar (50),
	@itemID int,
	@returnCheck int output
)
As 
Begin

	Declare @id int

	if exists
	(
		select *
		from orders
	)
	begin
		select @id = count(ID) + 1
		from Orders
	end

	else
	begin
		set @id = 1
	end

	declare @amountToBeDeducted int
	select @amountToBeDeducted = p.Price
	from product as p
	where p.ID = @itemID
	

	update UserInfo
	set Balance = Balance - @amountToBeDeducted
	where Email = @currentUserEmail

	if
	(
		select Balance
		from UserInfo
		where Email = @currentUserEmail
	) > -1
	begin
		Set @returnCheck = 1
		Update ProductQuantity
		Set Quantitiy = Quantitiy - 1
		Where productID = @itemID

		declare @userID int
		Select @userID = ID
		From UserInfo
		Where Email = @currentUserEmail

		Insert into orders values (@id, @userID, @itemID, getdate(), getdate() + 7)
		Insert into OrderDetails values (@id, @itemID, 1)
	end
	else
	begin
		set @returnCheck = 0
		Update UserInfo
		Set Balance = Balance + @amountToBeDeducted
		Where Email = @currentUserEmail
	
		print 'Current balance is insufficient!'
	end

End

declare @checkValue int
execute buyingItem 'haider@gmail.com',3,@returnCheck = @checkValue out
select @checkValue

Select * from UserInfo
go
-----4	Review a Product-----
Create procedure ReviewProduct
(
	@userID int,
	@itemID int,
	@review varchar (500), 
	@rating int
)
As 
Begin
	Declare @id int
	If exists (Select * From Reviews)
	Begin
		Select @id = count(*) + 1
		From Reviews
	End

	Else
	Begin
		Set @id = 1
	End

	If not exists 
	(
		Select *
		From Reviews
		Where Reviews.userID = @userID and Reviews.productID = @itemID
	)
	Begin
		Insert into Reviews values (@id, @userID, @rating, @itemID, @review)
	End

	Else 
		print 'Review already posted!'
	
	
end

Execute ReviewProduct 3,2,'Best CPU performance and good price',10
go

-----5	View Current User Details-----
Create Procedure currentUserDetails
(
	@currentUser varchar(50)
)
As
Begin
	If not Exists (Select * From UserInfo Where UserInfo.Email = @currentUser)
	Begin
		Print 'User doesnt Exist'
	End
	
	Else
	Begin
		Select *
		From UserInfo
		Where UserInfo.Email = @currentUser
	End
End

Exec currentUserDetails 'omer@gmail.com'
go

-----6	View Products Rating-----
Create procedure displayAvgRating
(
	@productID int
)
As
Begin
	Select avg(Reviews.Rating)
	From Reviews
	Where Reviews.productID = @productID
	Group by Reviews.productID
End
go

Execute displayAvgRating 2
go

-----7	Search a product-----
Create procedure Search
(
	@searchString varchar(50)
)
As
Begin
	If Exists
	(
		Select *
		From Product
		Where Product.Name like '%' + @searchString + '%'
	)
	Begin
		Select *
		From Product
		Where Product.Name like '%' + @searchString + '%'
	End
	
	Else
	Begin
		If Exists 
		(
			Select *
			From Product
			Where Product.Category like '%' + @searchString + '%'
		)
		Begin
			Select *
			From Product
			Where Product.Category like '%' + @searchString + '%'
		End
		Else
		Begin
			Select * 
			From Product
			Where Product.Id = 1000000
		End
	End
End

Execute Search 'Electronics'
go

-----8	Popular Products-----
Create procedure PopularProducts
as
begin
	Select Product.*
	From Product
	Inner join Reviews ON (Reviews.productID = Product.ID)
	Group by Product.ID, Product.Name, Product.Category, Product.Price
	Having Avg(Reviews.Rating) > 7

end
go

execute popularProducts
go

-----9	View Categories-----
Create procedure ViewCategories
As
Begin
	Select *
	From Category
	Order by Category.Name asc
End

Execute ViewCategories
go

-----10 Change Password-----
Create procedure ChangePassword
(
	@email varchar(50),
	@password varchar(16),
	@newPassword varchar(16),
	@returnCheck int OUTPUT
)
As
Begin
	If Exists
	(
		Select * 
		From UserInfo 
		Where UserInfo.Email = @Email and Userinfo.[Password] = @Password
	)
	Begin
		Update UserInfo
		Set UserInfo.[Password] = @newPassword
		Where UserInfo.[Password] = @password and UserInfo.Email = @email
		Set @returnCheck = 1

		Print 'Password changed successfully'
	End
	
	Else
	Begin
		Set @returnCheck = 0

		Print 'Password/Email Combination does not match'
	End
End
go

-----11 Read Reviews-----
go
Create procedure readReviews
(
	@itemID int
)
As 
Begin
	select r.userID, p.Name, r.[Description], r.Rating
	From Reviews as r
	Inner join Product as p on(p.ID = r.productID)
	where r.productID = @itemID
End

execute readReviews 1
go

----------Admin's Powers----------
-----12	Delete a Review-----
Create Procedure DeleteReview
(
	@reviewID int
)
As
Begin
	Delete 
	From Reviews
	Where Reviews.ID = @reviewID
End
Select * from Product

-----13	Add new Item in Store----
go
Create Procedure addNewItem
(
	@productCategory varchar (50),
	@productName varchar(50),
	@productPrice int,
	@productAmount int,
	@returnCheck int out
)
As
Begin
	declare @id int
	if exists
	(
		select *
		from Product
	)
	Begin
		Select top 1 @id = ID
		from Product
		Order by ID desc
	End

	Set @id = @id + 1

	if not exists
	(
		Select *
		From Product
		Where Product.Name = @productName
	)
	Begin
		Set @returnCheck = 1
		If not exists (Select * from Category Where Name Like @productCategory)
		Begin
			Insert into Category values (@productCategory)
		End
		
		Insert into Product values (@id, @productName, @productCategory, @productPrice)
		Insert into ProductQuantity values (@id, @productAmount)	
	End
	
	Else
	Begin
		Set @returnCheck = 0
		print 'Product Already in Store'
	End

End
go


-----14	Delete Item From Store-----
go
Create Procedure deleteItem
(
	@productName varchar(50),
	@returnCheck int out
)
As
Begin
	If Exists
	(
		Select * 
		From Product
		Where Product.Name = @productName
	)
	Begin
		Set @returnCheck = 1

		declare @productID int
		Select @productID = ID
		From Product
		Where Name = @productName

		Delete
		From dbo.Orders
		Where productID = @productID

		Delete
		From dbo.OrderDetails
		Where productID = @productID

		Delete
		From dbo.Reviews
		Where productID = @productID

		Delete
		From dbo.ShoppingCart
		Where productID = @productID

		Delete
		From ProductQuantity
		Where productID = @productID
		
		Delete 
		From Product
		Where ID = @productID

	End
	
	Else
	Begin
		Set @returnCheck = 0
	End
End
go

-----Password Length Trigger-----
Create trigger pass_length
On UserInfo
Instead of insert
As
	declare @pass varchar(20)
	begin
		select @pass=i.[password] from inserted as i
		if(len(@pass)<5)
			begin
				print('Error!!. Length of password should be greater than 4')
			end
		else
			begin
				declare @Id int
				Select @Id=i.ID from inserted as i
				declare @CName varchar(20)

				Select @CName=i.Name from inserted as i
				declare @CEmail varchar(30)
			
				Select @CEmail=i.[Password] from inserted as i
				declare @CPassword varchar(20)

				declare @bal int
				Select @bal=i.Balance from inserted as i

				declare @CAddress varchar(100)
				Select @CAddress=i.[Address] from inserted as i
				declare @CNumber varchar(50)
				Select @CNumber=i.PhoneNumber from inserted as i

				insert into UserInfo values(@Id,@CName,@CEmail,@CPassword,@bal,@CAddress,@CNumber)
			end

	end

go
-----Ordered Quanity Trigger-----
Create trigger Quantity
On OrderDetails
instead of insert
As
	begin
		declare @quan int
		declare @id int
		declare @check int

		select @quan=quantity,@id=productID from inserted
		select @check=pq.Quantitiy from ProductQuantity as pq where productID=@id 

		if(@check>@quan)
			begin
				insert into OrderDetails(orderID,productID,quantity)
				Select *
				from inserted

			end
		else
			begin
				print('Error!!. Items present are Only'+ @quan)
			end
	end


go
-----Date Trigger-----
Create trigger DateTrigger
on orders
instead of insert
As
	begin
		declare @odate date
		declare @ddate date

		select @odate=orderdate from inserted
		select @ddate=deliveryDate from inserted

		if(@odate>@ddate)
			begin
				print ('INVAlID ORDER DATE!!!!')
			end
		else
			begin
				insert into Orders(ID,userID,productID,orderDate,deliveryDate)
				Select *
				from inserted	
			end

	end
go

-----Product Quantity Trigger-----
Create trigger QuantityTrigger
on ProductQuantity
Instead of Insert
As
Begin
	declare @quan int
	select @quan=inserted.Quantitiy from inserted

	if(@quan<=0)
	begin
		print('Error!!. Quantity should be greater than 0')
	end

	else
	begin
		Insert into ProductQuantity(productID,Quantitiy)
		Select *
		From inserted
	end

End

Insert into ProductQuantity values (3,0)	
