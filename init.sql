Use Master
Go

Execute sp_configure 'show advanced options', 1;
Go

Execute sp_configure 'Agent XPs', 1;
Go

Execute sp_configure 'Replication XPs', 1;
Go

Execute sp_configure 'remote access', 1;
Go

Execute sp_configure 'Ole Automation Procedures', 1;
Go

Execute sp_configure 'external rest endpoint enabled', 1;
Go

Execute sp_configure 'external scripts enabled', 1;
Go

Reconfigure;
Go

Execute sp_configure 'show advanced options', 0;
Go

