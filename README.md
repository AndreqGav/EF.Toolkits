# EF.Toolkits

Данный проект содержит библиотеки для расширения возмжностей EF Core

## Регистрация
```
optionsBuilder
    .UseNpgsql("Host=localhost;Port=5432;Database=EF.Toolkits.Tests;Username=postgres;Password=****")
    .UseCustomSql(options => options.UseTriggers())
    .UseAutoComments("Comments.xml");
```

### EF.Toolkits.AutoComments
На основе файлов документации позволяет автоматически добавлять комментарии к столбцам и таблицам. Также поддерживает описание значений ```Enum``` через атрибут ```AutoCommentsEnumValues```. В проект необходимо добавить генерацию файла документации и указать его имя при подключении расширения.
```
    <PropertyGroup>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <DocumentationFile>Comments.xml</DocumentationFile>
    </PropertyGroup>
```

### EF.Toolkits.CustomSql
Добавляет возможность автоматически добавлять пользовательский SQL в код миграции с поддержкой изменения. Для каждого пользовательского SQL необхдимо определить 2 скрипта: на создание (Up) и на удаление (Down). Соответствующие скрипты попадут в файл миграции. В начало ммиграции добавляются скрпты для удаления, в конце миграции на добавление.
```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder
        .AddCustomSql("animals_view", "SELECT * FROM animals a WHERE a.type = 1", "DROP VIEW  IF EXISTS animals_view");
}
```

#### EF.Toolkits.CustomSql.Triggers.Postgresql
Провайдер для автоматического добавления триггеров. Зависит от ```EF.Toolkits.CustomSql```

```
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Figure>(entity =>
   {
       entity.BeforeInsert("set_square", "new.square = 0");

       entity.BeforeUpdate("prevent_update_with_negative_square", "IF new.square < 0 THEN raise exception 'square negative' END IF;");
    });
}
```

## Пример файла миграции
```
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SELECT * FROM animals a WHERE a.type = 1");

            migrationBuilder.Sql("CREATE FUNCTION prevent_update_with_negative_square() RETURNS trigger as $prevent_update_with_negative_square$\r\nBEGIN\r\nIF new.square < 0 THEN raise exception 'square negative' END IF;\r\nRETURN NEW;\r\nEND;\r\n$prevent_update_with_negative_square$ LANGUAGE plpgsql;\r\n\r\nCREATE TRIGGER prevent_update_with_negative_square BEFORE UPDATE\r\nON Figures\r\nFOR EACH ROW EXECUTE PROCEDURE prevent_update_with_negative_square();\r\n");

            migrationBuilder.Sql("CREATE FUNCTION set_square() RETURNS trigger as $set_square$\r\nBEGIN\r\nnew.square = 0\r\nRETURN NEW;\r\nEND;\r\n$set_square$ LANGUAGE plpgsql;\r\n\r\nCREATE TRIGGER set_square BEFORE INSERT\r\nON Figures\r\nFOR EACH ROW EXECUTE PROCEDURE set_square();\r\n");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW  IF EXISTS animals_view");

            migrationBuilder.Sql("DROP FUNCTION prevent_update_with_negative_square() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION set_square() CASCADE;");
        }
    }
```
