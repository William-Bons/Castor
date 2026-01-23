/* Diagniisis IN kind classification */
alter table movebook
add COLUMN Ai int GENERATED ALWAYS as (
iif(dsin == "21" or dsin=="22" or dsin=="23" or dsin =="25" or dsin=="30" or dsin =="31" or dsin =="32" or dsin=="00" or dsin=="01",1,0)
);

alter table movebook
add COLUMN Bi int GENERATED ALWAYS as (
iif(dsin == "20",1,0)
);

alter table movebook
add COLUMN Ci int GENERATED ALWAYS as (
iif(dsin == "70" or dsin =="71" or dsin=="72",1,0)
);

alter table movebook
add COLUMN Di int GENERATED ALWAYS as (
iif(dsin == "02" or dsin=="03" or dsin =="04" or dsin =="05" or dsin =="06" or dsin =="07" or dsin=="50" or dsin =="60" or dsin=="61" or dsin =="62" 
	or dsin=="90" or dsin =="91" or dsin =="40" or dsin =="41" or dsin =="42" or dsin=="43" or dsin =="45",1,0)
);

alter table movebook
add COLUMN Ei int GENERATED ALWAYS as (
iif(dsin == "10" or dsin =="15" or dsin =="19",1,0)
);

alter table movebook
add COLUMN contr_in TEXT GENERATED ALWAYS as (
iif(Ai==0 and Bi==0 and Ci==0 and Di==0 and Ei=0, "Error: diagnosis is not classified", NULL)
)

/* Diagniisis OUT kind classification */
alter table movebook
add COLUMN Ao int GENERATED ALWAYS as (
iif(dsout == "21" or dsout=="22" or dsout=="23" or dsout =="25" or dsout=="30" or dsout =="31" or dsout =="32" or dsout=="00" or dsout=="01",1,0)
);

alter table movebook
add COLUMN Bo int GENERATED ALWAYS as (
iif(dsout == "20",1,0)
);

alter table movebook
add COLUMN Co int GENERATED ALWAYS as (
iif(dsout == "70" or dsout =="71" or dsout=="72",1,0)
);

alter table movebook
add COLUMN 'Do' int GENERATED ALWAYS as (
iif(dsout == "02" or dsout=="03" or dsout =="04" or dsout =="05" or dsout =="06" or dsout =="07" or dsout=="50" or dsout =="60" or dsout=="61" or dsout =="62" 
	or dsout=="90" or dsout =="91" or dsout =="40" or dsout =="41" or dsout =="42" or dsout=="43" or dsout =="45",1,0)
);

alter table movebook
add COLUMN Eo int GENERATED ALWAYS as (
iif(dsout == "10" or dsout =="15" or dsout =="19",1,0)
);

alter table movebook
add COLUMN contr_out TEXT GENERATED ALWAYS as (
iif(Ao==0 and Bo==0 and Co==0 and 'Do'==0 and Eo=0, "Error: diagnosis is not classified", NULL)
)