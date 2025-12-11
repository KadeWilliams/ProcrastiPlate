# ProcrastiPlate Recipe tracker 

This is part portfolio project and part personal utility for me to be able to track recipes and things related to recipes. 

This is my first attempt at writing a project using docker containers. This is mainly to avoid the overhead of messing with databases and the ease of use in spinning up application dependencies. 

This is also the first time I've ever developed a .NET application using MacOS as the starting OS. That was a headache when combined with docker.

But I was able to get it to work by shear will.

just for when I inevitably forget the URL and port for the swagger and api https://localhost:5002/swagger/index.html
just for when I inevitably forget the URL and port for the swagger and api https://localhost:5148/swagger/index.html

Todo:
- [ ] Document what I did to get it working 
- [ ] Build out the initial CRUD operations through the API
- [ ] Build the landing page for the user. 
- [ ] Basic authentication however that might be 
- [ ] Show a users saved things

To get the docker container running for the database:
`docker run --name procrastiplate-postgres -e POSTGRES_DB=procrastiplate -e POSTGRES_USER=procrastiplateuser -e POSTGRES_PASSWORD=1234 -p 5432:5432 -v procrastiplate-data:/var/lib/postgresql/data -d postgres:16`

To get the database seeded with test data:
`docker exec -i procrastiplate-postgres psql -U procrastiplateuser -d procrastiplate < database-seed.sql`
