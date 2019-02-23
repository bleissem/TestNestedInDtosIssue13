# TestNestedInDtosIssue13

[![Build status](https://bleissem.visualstudio.com/githubpipeline/_apis/build/status/githubpipeline-ASP.NET%20Core%20(.NET%20Framework)-CI)](https://bleissem.visualstudio.com/githubpipeline/_build/latest?definitionId=79)

don't forget to use your own connection string in appsettings.json and publish SQL database project first

based on 
[TestNestedInDtosIssue13](https://github.com/bleissem/EfCore.GenericServices/blob/master/Tests/UnitTests/GenericServicesPublic/TestNestedInDtosIssue13.cs)
and [issue 13](https://github.com/JonPSmith/EfCore.GenericServices/issues/13)
I faced a problem when __only__ updating nested dto's 


The problem as far as I analyzed it is that in 

[CreateMapper](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/GenericServices/Internal/MappingCode/CreateMapper.cs)

the function 
```
            public TEntity ReturnExistingEntity(object[] keys)
            {
                return _context.Set<TEntity>().Find(keys);
            }
```
___does not load its navigation properties___ when only updating a dto as shown in 
ValuesController when calling the function service.UpdateAndSave
```
            InContactAddressDto updateDTO = service.ReadSingle<InContactAddressDto>(w => w.ContactAddressId == contactAddressId);

            updateDTO.Name = "test2";
            updateDTO.Addess.Address1 = "other street";

            service.UpdateAndSave(updateDTO);

```

This causes the mapper to think a new row has to be inserted and generates a insert sql. 

This causes the error __SqlException: Violation of PRIMARY KEY constraint__ 

The error does not occur when first inserting and then updating the nested dto in the same function.

