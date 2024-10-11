﻿using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace EF.Toolkits.Shared
{
    public interface IMigrationOperationModifier
    {
        IReadOnlyList<MigrationOperation> ModifyOperations(IReadOnlyList<MigrationOperation> operations,
            IRelationalModel source,
            IRelationalModel target
        );
    }
}