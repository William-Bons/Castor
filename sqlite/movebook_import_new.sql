
--DELETE from Movebooks;

INSERT into Movebooks (card_id, fio,birthdate,datein,dateout,ordered,dsin,dsout,outto,city,first,second,early,unvoluntary,date_lastout) 
SELECT card_id, fio,date(birthdate),date(datein),date(dateout),ordered,dsin,dsout,outto,city,first,second,early,unvoluntary,date(date_lastout) FROM '010126';