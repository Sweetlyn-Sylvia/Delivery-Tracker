create table Supervisor(
SupervisorId nvarchar(20) not null primary key,
Name nvarchar(50) not null,
Email nvarchar(50) not null,
Phone nvarchar(15) not null,
Password nvarchar(50) not null
)



select*from Supervisor

alter procedure sp_LoginSupervisor(
@SupervisorId nvarchar(20),
@Password nvarchar(50)
)
as
begin
begin try

if trim(@SupervisorId)=''
begin
select 'Supervisor Id cannot be empty' as Message
return
end

if trim(@Password)=''
begin
select 'Password cannot be empty' as Message
return
end

if not exists(select 1 from Supervisor where SupervisorId=@SupervisorId)
begin
select 'Supervisor Id cannot be found'
return
end

if not exists(select 1 from Supervisor where SupervisorId=@SupervisorId and Password=HASHBYTES('SHA2_256',@Password))
begin
select 'Invalid credentials' as Message
return
end

select
'Login Successful' as Message
from Supervisor
where SupervisorId=@SupervisorId

end try
begin catch
select 'Error during supervisor login' as Message
end catch
end

--Dashboard Summary

alter procedure sp_GetDashboardSummary
as
begin
begin try

declare @Today Date=cast(getdate() as Date)

select
(select count(*) from Parcels where cast(Date as Date)=@Today) as TotalParcels,

(select count(*) from Parcels where status='Delivered' and cast(StatusUpdatedDate as Date)=@Today) as DeliveredParcels,

(select count(*) from Parcels where status='In Transit' and cast(StatusUpdatedDate as Date)=@Today) as InTransitParcels,

(select count(*) from Parcels where status='Picked Up' and cast(Date as Date)=@Today) as PickedUpParccels,

(select count(*) from Agents) as TotalAgents,

(select isnull(sum(DeliveryAmount),0)
from Parcels where cast(Date as Date)=@Today) as TodaysEarning

end try
begin catch
select 'Error while retrieving dashboard data' as Message
end catch
end

--Get Supervisor Profile procedure

create procedure sp_GetSupervisorById
(
@SupervisorId nvarchar(20)
)
as
begin
begin try

if not exists(select 1 from Supervisor where SupervisorId=@SupervisorId)
begin
select 'Supervisor not found' as Message
return
end

select
SupervisorId,
Name,
Email,
Phone
from Supervisor
where SupervisorId=@SupervisorId

end try
begin catch
select 'Error while retrieving supervisor profile' as Message
end catch
end


--Update Supervisor Profile

alter procedure sp_UpdateSupervisorProfile
(
@SupervisorId nvarchar(20),
@Name nvarchar(50),
@Email nvarchar(50),
@Phone nvarchar(15),
@CurrentPassword nvarchar(50),
@NewPassword nvarchar(50)
)
as
begin
begin try

if not exists (select 1 from Supervisor where SupervisorId=@SupervisorId)
begin
select 'Supervisor not found' as Message
return
end

if @Email is null or @Email not like '%_@_%._%'
begin
select 'Invalid Email format' as Message
return
end

if len(@Phone)<>10 or @Phone  like '%[^0-9]%'
begin
select 'Phone number must be exactly 10 digits' as Message
return 
end

if isnull(@CurrentPassword,'') <> '' and isnull(@NewPassword,'') <> ''
begin
if not exists(select 1 from Supervisor where SupervisorId=@SupervisorId and Password=HASHBYTES('SHA2_256',@CurrentPassword))
begin
select 'Currrent password is incorrect' as Message
return
end

if len(@NewPassword)<8
or @NewPassword not like '%[A-Z]%'
or @NewPassword not like '%[a-z]%'
or @NewPassword not like '%[0-9]%'
or @NewPassword not like '%[^A-Za-Z0-9]%'

begin
select 'Password must contain minimum 8 characters with uppercase,lowercase,number and special charcter' as Message
return
end

Update Supervisor
set Password=HASHBYTES('SHA2_256',@NewPassword)
where SupervisorId=@SupervisorId

end

update Supervisor
set Name=@Name,
Email=@Email,
Phone=@Phone
where SupervisorId=@SupervisorId

select 'Profile updated successfully' as Message

end try
begin catch
select 'Error while updating profile' as Message
end catch
end

--Delayed Notification procedure

create procedure sp_GetSupervisorNotifications
as
begin
begin try

declare @Now datetime=getdate()
select
p.ParcelId,
p.Status,
p.AgentId,
a.AgentName as AgentName,
'Delayed' as Type,
'Parcel is delayed in picked up state more than 24 hours' as Message
from Parcels p
left join Agents a on a.AgentId=p.AgentId
where p.Status='Picked Up'
and datediff(hour,p.Date,@Now)>=24

union

select
p.ParcelId,
p.Status,
p.AgentId,
a.AgentName as AgentName,
'Delayed',
'Parcel is delayed in in transit state more than 72 hours' 
from Parcels p
left join Agents a on a.AgentId=p.AgentId
where p.Status='In Transit'
and datediff(hour,p.Date,@Now)>=72

union

select ParcelId,
Status,
Null,
'Not Assigned',
'Unassigned',
'Parcel has no agent assigned'
from Parcels
where AgentId is null

end try
begin catch
select 'Error retrieving notifications' as Message
end catch
end


--Assign agent to parcel

create procedure sp_AssignAgentToParcel
(
@ParcelId nvarchar(20),
@AgentId nvarchar(20)
)
as
begin
begin try

if not exists(select 1 from Parcels where ParcelId=@ParcelId)
begin
select 'Parcel not found' as Message
return
end

if not exists(select 1 from Agents where AgentId=@AgentId)
begin
select 'Agent not found' as Message
return
end

update Parcels
set AgentId=@AgentId where ParcelId=@ParcelId

select 'Agent assigned successfully' as Message

end try
begin catch
select 'Error assigning agent' as Message
end catch
end
--Agent Email

create procedure sp_GetAgentEmail
(
    @AgentId nvarchar(20)
)
as
begin
begin try

select Email
from Agents
where AgentId = @AgentId

end try

begin catch
select 'Error retrieving agent email' as Message
end catch
end





---Report Generation

create procedure sp_GetDailyReport
as
begin

select
ParcelId,
SenderName,
ReceiverName,
AgentId,
Status,
ReceiverContactNumber,
Date,
Remarks,
DeliveryAmount
from Parcels
where cast(Date as DATE)=cast(Getdate() as DATE)
end

create procedure sp_GetDeliveredReport
as
begin

select
ParcelId,
SenderName,
ReceiverName,
AgentId,
Status,
ReceiverContactNumber,
Date,
Remarks,
DeliveryAmount
from Parcels
where Status='Delivered' and cast(Date as DATE)=cast(Getdate() as DATE)
end

create procedure sp_GetPendingReport
as
begin

select
ParcelId,
SenderName,
ReceiverName,
AgentId,
Status,
ReceiverContactNumber,
Date,
Remarks,
DeliveryAmount
from Parcels
where Status='Picked Up' or Status='In Transit' 
end

ALTER PROCEDURE sp_GetRangeReport
(
    @StartDate DATETIME,
    @EndDate DATETIME
)
AS
BEGIN

SELECT
ParcelId,
SenderName,
ReceiverName,
AgentId,
Status,
ReceiverContactNumber,
Date,
Remarks,
DeliveryAmount,
SUM(DeliveryAmount) OVER() AS TotalDeliveryAmount
FROM Parcels
WHERE Date BETWEEN @StartDate AND @EndDate

END
create procedure sp_GetAgentReport
@AgentId nvarchar(20)
as
begin

select
ParcelId,
SenderName,
ReceiverName,
AgentId,
Status,
ReceiverContactNumber,
Date,
Remarks,
DeliveryAmount
from Parcels
where AgentId=@AgentId
end



select * from Agents




select * from Supervisor





