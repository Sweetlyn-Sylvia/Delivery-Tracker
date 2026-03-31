create table Parcels
(
ParcelId nvarchar(20) primary key,
SenderName nvarchar(100) not null,
ReceiverName varchar(100) not null,
SenderAddress nvarchar(200) not null,
ReceiverAddress nvarchar(200) not null,

ReceiverContactNumber nvarchar(15) not null,
Weight decimal(10,2) not null,
Date datetime default getdate(),
Status nvarchar(30) default 'Picked Up',
AgentId nvarchar(20),
DeliveryTime datetime null,
Remarks nvarchar(200),
DeliveryAmount decimal(10,2),
FastDelivery bit default 0,
PaymentMode nvarchar(50),
IsPaid bit default 0,
CurrentLat float null,
CurrentLng float null,
LocationUpdatedAt datetime null,
CurrentLocationText nvarchar(100),
IsActive bit default 1
)
--Foreign Key
ALTER TABLE Parcels
ADD CONSTRAINT FK_Parcels_Agent
FOREIGN KEY (AgentID)
REFERENCES Agents(AgentId)

--Create Parcel Procedure

create procedure sp_CreateParcel(
@ParcelId nvarchar(20),
@SenderName nvarchar(100),
@ReceiverName nvarchar(100),
@SenderAddress nvarchar(200),
@ReceiverAddress nvarchar(200),
@ReceiverContactNumber nvarchar(15),
@Weight decimal(10,2),
@AgentId nvarchar(20),
@DeliveryAmount decimal(10,2),
@FastDelivery bit,
@PaymentMode nvarchar(50),
@IsPaid bit
)
as
begin
begin try

if trim(@SenderName)=''
begin
select 'Sender name cannot be empty' as Message
return
end

if trim(@ReceiverName)=''
begin
select 'Receiver name cannot be empty' as Message
return
end

if len(@ReceiverContactNumber) <>10
or @ReceiverContactNumber like '%[^0-9]%'
begin
select 'Invalid contact number' as Message
return
end

if trim(@SenderAddress)=''
begin
select 'Sender address cannot be empty' as Message
return
end

if trim(@ReceiverAddress)=''
begin
select 'Receiver Address cannot be empty' as Message
return
end

insert into Parcels
(ParcelId,SenderName,ReceiverName,SenderAddress,ReceiverAddress,
ReceiverContactNumber,Weight,AgentId,DeliveryAmount,FastDelivery,PaymentMode,IsPaid,Status,Date)
values
(@ParcelId,@SenderName,@ReceiverName,@SenderAddress,@ReceiverAddress,
@ReceiverContactNumber,@Weight,@AgentId,@DeliveryAmount,
@FastDelivery,@PaymentMode,@IsPaid,'Picked Up',getdate())


select 'Parcel created successfully' as Message
end try
begin catch
select 'Error while creating parcel' as Message
end catch
end


--Get parcels Procedures

create procedure sp_GetAllParcels
as
begin
begin try
if not exists(select 1 from Parcels)
begin
select 'No parcels available' as Message
return
end

select*from Parcels
end try

begin catch
select 'Error retrieving parcels' as Message
end catch
end

--Get Parcel By Id procedure

create procedure sp_GetParcelById
(
@ParcelId nvarchar(20)
)
as
begin
begin try
if not exists(select 1 from Parcels where ParcelId=@ParcelId)
begin
select 'No parcel found' as Message
return
end

select * from Parcels where ParcelId=@ParcelId
end try
begin catch
select 'Error retrieving parcels' as Message
end catch
end

--Update Parcel Status procedure

alter procedure sp_UpdateParcelStatus
(
@ParcelId nvarchar(20),
@Status nvarchar(30),
@Remarks nvarchar(200)

)
as
begin
begin try
if not exists(select 1 from Parcels where ParcelId=@ParcelId)
begin
select 'No parcel found' as Message
return
end

UPDATE Parcels
SET
Status=@Status,
Remarks=@Remarks,
StatusUpdatedDate = GETDATE(),

DeliveryTime = CASE 
                WHEN @Status='Delivered' THEN GETDATE() 
                ELSE NULL 
               END
WHERE ParcelId=@ParcelId
select 'Parcel Status updated successfully' as Message

end try
begin catch
select 'Error updating parcel status' as Message
end catch
end

--Parcel Pending procedure

CREATE PROCEDURE sp_GetPendingParcels
AS
BEGIN
SELECT *
FROM Parcels
WHERE Status <> 'Delivered'
END

--Filter Parcels
ALTER PROCEDURE sp_FilterParcels
(
    @Status NVARCHAR(50) = NULL,
    @AgentID NVARCHAR(20) = NULL,
    @Date DATE = NULL
)
AS
BEGIN

SELECT 
    ParcelId,
    SenderName,
    ReceiverName,
    SenderAddress,
    ReceiverAddress,
    ReceiverContactNumber,
    Weight,
    Status,
    AgentId,
    Date,
    Remarks,
    FastDelivery
FROM Parcels
WHERE
(@Status IS NULL OR Status = @Status)
AND (@AgentID IS NULL OR AgentID = @AgentID)
AND (@Date IS NULL OR CAST(Date AS DATE) = @Date)

END
--UpdateParcelLocation Procedure

CREATE PROCEDURE sp_UpdateParcelLocation
(
@ParcelId NVARCHAR(20),
@Lat FLOAT,
@Lng FLOAT,
@LocationText NVARCHAR(200)
)
AS
BEGIN
BEGIN TRY

IF NOT EXISTS (SELECT 1 FROM Parcels WHERE ParcelId=@ParcelId)
BEGIN
SELECT 'Parcel not found' AS Message
RETURN
END

if trim(@ParcelId)=''
begin
select 'ParcelId cannot be empty' as Message
return
end
UPDATE Parcels
SET
CurrentLat=@Lat,
CurrentLng=@Lng,
CurrentLocationText=@LocationText,
LocationUpdatedAt=GETDATE()
WHERE ParcelId=@ParcelId

SELECT 'Location updated successfully' AS Message

END TRY

BEGIN CATCH
SELECT 'Error updating parcel location' AS Message
END CATCH
END

--Track Parcel Procedure

alter PROCEDURE sp_TrackParcel
(
@ParcelId NVARCHAR(20)
)
AS
BEGIN
BEGIN TRY

IF NOT EXISTS (SELECT 1 FROM Parcels WHERE ParcelId=@ParcelId)
BEGIN
SELECT 'Parcel not found' AS Message
RETURN
END
if trim(@ParcelId)=''
begin
select 'ParcelId cannot be empty' as Message
return
end

SELECT
*
FROM Parcels
WHERE ParcelId=@ParcelId

END TRY

BEGIN CATCH
SELECT 'Error retrieving parcel tracking details' AS Message
END CATCH
END

select * from Agents
SELECT FastDelivery FROM Parcels


--Parcel by Agent Id

CREATE PROCEDURE sp_GetParcelsByAgentId
@AgentId VARCHAR(50)
AS
BEGIN
SELECT *
FROM Parcels
WHERE AgentId = @AgentId
END
