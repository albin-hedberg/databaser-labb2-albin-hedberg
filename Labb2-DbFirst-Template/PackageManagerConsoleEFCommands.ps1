﻿#Scaffold-DbContext -Connection "Data Source=DESKTOP-KAHUNTM;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir "Entities"
Scaffold-DbContext -Connection "Data Source=DESKTOP-KAHUNTM;Initial Catalog=everyloop;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir "DataAccess\Entities"
Scaffold-DbContext -Connection "Data Source=DESKTOP-KAHUNTM;Initial Catalog=everyloop;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir "DataAccess\Entities" -ContextDir "DataAccess" -Schema "music"
# -Force skriver över tidigare filer
Scaffold-DbContext -Connection "Data Source=DESKTOP-KAHUNTM;Initial Catalog=everyloop;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" -Provider "Microsoft.EntityFrameworkCore.SqlServer" -OutputDir "DataAccess\Entities" -ContextDir "DataAccess" -Schema "music" -Force