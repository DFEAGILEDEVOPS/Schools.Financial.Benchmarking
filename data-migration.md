# SFB Deployment

## DocumentDB collection import

Under `deploy\docdb` is the Data transfer tool and associated batch files.

### _config.bat

This is where you specify the source and destination DocumentDB instances.  Only edit the first 2 sections as the 3rd one is where the final connection string is constructed.

### collections.txt

This is a simple list of the collections that need to be migrated from source to destination.  If you modify this to reduce the number of collections migrated, ensure you do not check in the change.

### import-collection.bat

This simply constructs the call to be executed against each collection.  Do not execute it directly, as it is included in the main migration script.

### migrate.bat

This is the main migration script and is what you execute to run the whole migration.  It depends on `_config.bat`, `collections.txt` and `import-collection.bat`.

### Running a data migration

1. Delete the existing collections from database using Azure portal(or delete the DB and create a new one with the same name)
2. Update _config.bat with your source and target databases.
3. Ensure collections.txt contains a list of the collections you want to migrate.
4. Run migrate.bat via the command line on a windows machine.
5. You will receive feedback throughout the migration on each collection.
