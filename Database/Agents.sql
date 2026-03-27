create TABLE Agents(
AgentId NVARCHAR(20) PRIMARY KEY,
AgentName NVARCHAR(100) NOT NULL,
Email NVARCHAR(100)  NOT NULL,
Phone NVARCHAR(15),
Password NVARCHAR(100) NOT NULL,
IsActive BIT DEFAULT 1
)
--Register Agent Procedure

alter procedure sp_RegisterAgent(
@AgentId nvarchar(20),
@AgentName nvarchar(100),
@Email nvarchar(100),
@Phone nvarchar(15),
@Password nvarchar(100)
)
as
begin
begin try
--Name validation
if trim(@AgentName)=''
begin
select 'Agent name cannot be empty' as Message 
return
end
if @AgentName like '%[^A-Za-z ]%'
begin
select 'Agent Name should have only letters and spaces' as Message
return
end

--Password Validation
if trim(@Password)=''
begin
select 'Password cannot be empty' as Message
return 
end

if len(@Password)<8
or @Password not like '%[A-Z]%'
or @Password not like '%[a-z]%'
or @Password not like '%[0-9]%'
or @Password not like '%[^A-Za-z0-9]%'

begin
select 'Invalid Password format' as Message
return
end

--Email Validation
if @Email not like '%_@_%._%'
begin
select 'Invalid Email format' as Message
return
end

if exists(select 1 from Agents where Email=@Email and IsActive=1)
begin
select 'email already exists' as Message
return
end



--Phone validation

if trim(@Phone)=''
begin
select 'phone number cannot be empty' as Message
return
end



if len(@Phone) <> 10
begin
select 'Phone number must be 10 digits' as Message
return
end

if @Phone like '%[^0-9]%'
begin
select 'Phone number must contain only numbers' as Message
return 
end

if exists (select 1 from Agents where Phone=@Phone and IsActive=1)
begin
select 'Phone number already exists' as Message
return
end

--Insert Values

insert into Agents(AgentId,AgentName,Email,Phone,Password)
values(@AgentId,@AgentName,@Email,@Phone,HASHBYTES('SHA2_256', @Password))
select @AgentId as AgentId,'Agent registered successfully' as Message
end try
begin catch
select 'Error while registering agent' as Message
end catch
end



--Login Agent Procedure

alter procedure sp_LoginAgent
(
    @Email NVARCHAR(100),
    @Password NVARCHAR(100)
)
as
begin
begin try
if (@Email IS NULL OR TRIM(@Email) = '')
and (@Password IS NULL OR TRIM(@Password) = '')
begin
    select 
        NULL AS AgentId,
        NULL AS AgentName,
        'Enter the email and password' AS Message
    return
end

declare @AgentId NVARCHAR(20)
declare @AgentName NVARCHAR(100)

select 
    @AgentId = AgentId,
    @AgentName = AgentName
from Agents
where 
    Email = @Email
AND Password = HASHBYTES('SHA2_256', CONVERT(NVARCHAR(100), @Password))
AND IsActive = 1

if @AgentId IS NOT NULL
begin
select
        @AgentId AS AgentId,
        @AgentName AS AgentName,
        'Agent logged in successfully' AS Message
end
else
begin
    select
        NULL AS AgentId,
        NULL AS AgentName,
        'Incorrect email or password' AS Message
end
end try
begin catch
select
        NULL AS AgentId,
        NULL AS AgentName,
        'Login failed' AS Message
end catch
end




--Get all agents Procedures

create procedure sp_GetAllAgents
as
begin

begin try
if not exists (select 1 from Agents where IsActive =1)
begin
select 'No Agents Available' as Message
return
end

select AgentId,AgentName,Email,Phone
from Agents
where IsActive=1

end try
begin catch
select 'Unable to retrieve' as Message
end catch
end

--GetAgentById Procedure
alter procedure sp_GetAgentById(
@AgentId nvarchar(100)
)
as
begin
begin try

if not exists (select 1 from Agents where AgentId=@AgentId and IsActive =1)
begin
select 
NULL as AgentId,
NULL as AgentName,
NULL as Email,
NULL as Phone,
'Agent not found' as Message
return
end

select 
AgentId,
AgentName,
Email,
Phone,
'Agent retrieved successfully' as Message
from Agents
where AgentId=@AgentId and IsActive=1

end try
begin catch
select 
NULL as AgentId,
NULL as AgentName,
NULL as Email,
NULL as Phone,
'Unable to retrieve' as Message
end catch
end
--Delete Agent

alter procedure sp_DeleteAgent(
@AgentId nvarchar(100)
)
as 
begin
begin try

if not exists(select 1 from Agents where AgentId=@AgentId and IsActive=1)
begin
select 'Agent not found' as Message
return 
end

update Parcels
set AgentId=null
where AgentId=@AgentId

update Agents
set
IsActive=0
where AgentId=@AgentId

select 'Agent removed successfully'  as Message

end try
begin catch
select 'Error occured during deletion' as Message
end catch
end

--Agent Performance

alter procedure sp_GetAgentPerformance
as
begin
begin try

if not exists(select 1 from Agents where IsActive = 1)
begin
    select 'No agents available' as message
    return
end

if not exists(select 1 from Parcels)
begin
    select 'No parcel records available' as message
    return
end

select 
    a.AgentId as agentId,
    a.AgentName as name,

    sum(case when p.Status = 'Delivered' then 1 else 0 end) as delivered,
    sum(case when p.Status = 'In Transit' then 1 else 0 end) as inTransit,
    sum(case when p.Status = 'Picked Up' then 1 else 0 end) as pickedUp

from Agents a
left join Parcels p 
on a.AgentId = p.AgentId

where a.IsActive = 1

group by a.AgentId, a.AgentName

end try

begin catch
select 'Error retrieving agent performance' as message
end catch

end

--Agent Dashboard Summary

alter procedure sp_GetAgentDashboard
(
    @AgentId nvarchar(20)
)
as
begin
begin try

-- AgentId validation
if @AgentId is null or trim(@AgentId) = ''
begin
    select 'AgentId cannot be empty' as Message
    return
end

-- Check if agent exists
if not exists(select 1 from Agents where AgentId = @AgentId and IsActive = 1)
begin
    select 'Agent not found' as Message
    return
end

declare @Today date = cast(getdate() as date)
declare @Tomorrow date = dateadd(day,1,@Today)

-- Today's parcel statistics
select 
    
    count(case 
        when Date >= @Today and Date < @Tomorrow 
        then 1 end) as CreatedToday,

   
    sum(case 
        when Status = 'Picked Up' 
        and StatusUpdatedDate >= @Today and StatusUpdatedDate < @Tomorrow 
        then 1 else 0 end) as PickedUpToday,

    sum(case 
        when Status = 'In Transit' 
        and StatusUpdatedDate >= @Today and StatusUpdatedDate < @Tomorrow 
        then 1 else 0 end) as InTransitToday,

    sum(case 
        when Status = 'Delivered' 
        and StatusUpdatedDate >= @Today and StatusUpdatedDate < @Tomorrow 
        then 1 else 0 end) as DeliveredToday

into #TodayStats
from Parcels
where AgentId = @AgentId


-- ALL pending parcels (not today)
declare @PendingCount int

select @PendingCount = count(*)
from Parcels
where AgentId = @AgentId
and Status in ('Picked Up','In Transit')


-- Final result
select 
    CreatedToday,
    PickedUpToday,
    InTransitToday,
    DeliveredToday,
    @PendingCount as PendingCount
from #TodayStats

drop table #TodayStats

end try

begin catch
    select 'Error retrieving dashboard data' as Message
end catch
end


select* from Parcels where AgentId='AGT445CE8'
select * from Agents




update Parcels set AgentId=null where ParcelId='PARF86F448C'

select*from Agents















