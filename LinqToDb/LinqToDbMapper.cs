using LinqToDB.Mapping;

namespace LinqToDb
{
    public class LinqToDbMapper
    {
        private static MappingSchema schema = null;

        public static MappingSchema GetSchema()
        {
            if (schema == null)
            {
                schema = MappingSchema.Default;
                var mappingBuilder = schema.GetFluentMappingBuilder();

                mappingBuilder.Entity<City>()
                    .HasTableName("City")
                    .HasSchemaName("public")
                    .HasIdentity(x => x.Id)
                    .HasPrimaryKey(x => x.Id)
                    .HasColumn(x => x.Name)
                    .Ignore(x => x.MenuDisplay);
                    //.HasAttribute(new AssociationAttribute()
                    // {
                    //     ThisKey = "Wf",
                    //     OtherKey = "Id",
                    //     CanBeNull = true,
                    //     Relationship = Relationship.ManyToOne,
                    //     KeyName = "FK_City_WfId",
                    //     BackReferenceName = "CityWfIds"
                    // }
                    //);

                mappingBuilder.Entity<User>()
                    .HasTableName("User")
                    .HasSchemaName("public")
                    .HasIdentity(x => x.Id)
                    .HasPrimaryKey(x => x.Id)
                    .HasColumn(x => x.Login)
                    .HasColumn(x => x.Password)
                    .HasColumn(x => x.NsiAccesses)
                    .HasColumn(x => x.Email)
                    .HasColumn(x => x.IPAddress)
                    .HasColumn(x => x.IsAdmin)
                    .HasColumn(x => x.AuthToken)
                    .HasColumn(x => x.Name)
                    .HasColumn(x => x.DashboardIdToDisplay);

                mappingBuilder.Entity<BaseEntity>()
                    .HasTableName("BaseEntity")
                    .HasSchemaName("public")
                    .HasIdentity(x => x.Id)
                    .HasPrimaryKey(x => x.Id)
                    .HasColumn(x => x.CreatedWhen)
                    .HasColumn(x => x.ModifiedWhen)
                    .HasColumn(x => x.IsDeleted)
                    .Inheritance(x => x.Type, "WeatherForecast", typeof(WeatherForecast))
                    .Property(x => x.CreatedByUser)
                    .HasAttribute(new AssociationAttribute() { ThisKey = "CreatedByUserId", OtherKey = "Id", Relationship = Relationship.ManyToOne });
                    //.Association(x => x.ModifiedByUser, x => x.ModifiedByUser.Id, x => x.Id);

                mappingBuilder.Entity<WeatherForecast>()
                    .HasTableName("BaseEntity")
                    .HasSchemaName("public")
                    .HasIdentity(x => x.Id)
                    .HasPrimaryKey(x => x.Id)
                    .HasColumn(x => x.Date);
            }
            return schema;
        }
    }
}
