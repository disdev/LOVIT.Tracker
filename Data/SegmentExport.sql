select CONCAT(p.FirstName, ' ', p.LastName), p.Age, p.Gender, p.Rank, r.Code, s.[Order], s.Distance, s.TotalDistance, 0 as 'Segment Elapsed', ci.Elapsed
from Checkins ci
join Participants p on ci.ParticipantId = p.Id
join Segments s on ci.SegmentId = s.Id
join Races r on p.RaceId = r.Id
order by p.LastName, p.FirstName, s.[Order]