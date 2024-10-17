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
На основе файлов документации позволяет автоматически добавлять комментарии к столбцам и таблицам. В проект необходимо добавить генерацию файла документации и указать его имя при подключении расширения.
```
    <PropertyGroup>
        <GenerateDocumentationFile>true</GenerateDocumentationFile>
        <DocumentationFile>Comments.xml</DocumentationFile>
    </PropertyGroup>
```
Также поддерживается описание значений ```Enum``` через атрибут ```AutoCommentsEnumValues``` или с использованием настройки ```AddEnumValuesComments```
```
            optionsBuilder
                .UseAutoComments(options => options.WithXmlPaths("Comments.xml").AddEnumValuesComments());
```
Для того, чтобы исключить описание значений ```Enum``` можно использовать атрибут ```IgnoreAutoCommentsEnumValues```
Пример:
Для перечисления ```AnimalType``` будет создан такой комментарий: ```Тип.\n\n0 - Собакен.\n1 - Кошька.\n2 - Рыбка.\n3 - Другое.```
```
    /// <summary>
    /// Тип живтоного.
    /// </summary>
    public enum AnimalType
    {
        /// <summary>
        /// Собакен.
        /// </summary>
        Dog,

        /// <summary>
        /// Кошька.
        /// </summary>
        Cat,

        /// <summary>
        /// Рыбка.
        /// </summary>
        Fish,

        /// <summary>
        /// Другое.
        /// </summary>
        Other
    }
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
Чтобы автоматически обновлять свои SQL скрипты при обновлении модели можно использовать класс ```CustomSqlGenerator```, который имеет полезные функции для получения имени столбца и таблицы.



Пример:



Создаем класс ```TriggersGenerator``` в котором описываем свои SQL:
```
    public class TriggersGenerator : CustomSqlGenerator
    {
        public TriggersGenerator(DbContext dbContext, ModelBuilder modelBuilder) : base(dbContext, modelBuilder)
        {
        }

        public string GenerateTriggersScript()
        {
            var animal = GetTableName<Animal>();

            var species = GetColumnName<Animal>(x => x.Species);
            var animalType = GetColumnName<Animal>(x => x.AnimalType);

            return
                $"IF NEW.{species} IS NOT NULL AND NEW.{species} IS DISTINCT FROM OLD.{species} THEN\n" +
                $"    RAISE EXCEPTION 'Нельзя менять породу';\n" +
                $"END IF;\n" +
                $"IF NEW.{species} IS NOT NULL THEN\n" +
                $"    UPDATE {animal} SET {animalType} = NEW.{animalType}\n" +
                $"    WHERE {species} = NEW.{species};\n" +
                $"END IF;";
        }
    }
```
И используем его в ```OnModelCreating(ModelBuilder modelBuilder)```
```
            modelBuilder.Entity<Animal>(entity =>
            {
                var triggersGenerator = new TriggersGenerator(this, modelBuilder);

                entity.BeforeInsertOrUpdate("before_insert_or_update", triggersGenerator.GenerateTriggersScript());
            });
```
Если поля `Species`, `AnimalType` будут переименованы в БД, то при создании миграции будут обновлены ваши SQL.

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
Поддерживаются операции `Insert`, `Update`, `Delete`, `InsertOrUpdate` с временем выполнения триггера `Before`, `After`, `Instead`


Вам нужно только определить тело триггера, всё остальное сделает библиотека

## Пример файла миграции
```
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SELECT * FROM \"Animals\" a WHERE a.\"AnimalType\" = 1");

            migrationBuilder.Sql("CREATE FUNCTION before_insert_or_update() RETURNS trigger as $before_insert_or_update$\r\nBEGIN\r\nIF NEW.\"Species\" IS NOT NULL AND NEW.\"Species\" IS DISTINCT FROM OLD.\"Species\" THEN\n    RAISE EXCEPTION 'Нельзя менять породу';\nEND IF;\nIF NEW.\"Species\" IS NOT NULL THEN\n    UPDATE \"Animals\" SET \"AnimalType\" = NEW.\"AnimalType\"\n    WHERE \"Species\" = NEW.\"Species\";\nEND IF;\r\nRETURN NEW;\r\nEND;\r\n$before_insert_or_update$ LANGUAGE plpgsql;\r\n\r\nCREATE TRIGGER before_insert_or_update BEFORE INSERT OR UPDATE\r\nON \"Animals\"\r\nFOR EACH ROW EXECUTE PROCEDURE before_insert_or_update();\r\n");

            migrationBuilder.Sql("CREATE FUNCTION prevent_update_with_negative_square() RETURNS trigger as $prevent_update_with_negative_square$\r\nBEGIN\r\nIF new.square < 0 THEN raise exception 'square negative'; END IF;\r\nRETURN NEW;\r\nEND;\r\n$prevent_update_with_negative_square$ LANGUAGE plpgsql;\r\n\r\nCREATE TRIGGER prevent_update_with_negative_square BEFORE UPDATE\r\nON \"Figures\"\r\nFOR EACH ROW EXECUTE PROCEDURE prevent_update_with_negative_square();\r\n");

            migrationBuilder.Sql("CREATE FUNCTION set_square() RETURNS trigger as $set_square$\r\nBEGIN\r\nnew.square = 0;\r\nRETURN NEW;\r\nEND;\r\n$set_square$ LANGUAGE plpgsql;\r\n\r\nCREATE TRIGGER set_square BEFORE INSERT\r\nON \"Figures\"\r\nFOR EACH ROW EXECUTE PROCEDURE set_square();\r\n");
               }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW  IF EXISTS animals_view");

            migrationBuilder.Sql("DROP FUNCTION before_insert_or_update() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION prevent_update_with_negative_square() CASCADE;");

            migrationBuilder.Sql("DROP FUNCTION set_square() CASCADE;");
        }
    }
```
