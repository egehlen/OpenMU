// <copyright file="GameMapDefinition.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.GameMapDefinition"/>.
/// </summary>
[Table(nameof(GameMapDefinition), Schema = SchemaNames.Configuration)]
internal partial class GameMapDefinition : MUnique.OpenMU.DataModel.Configuration.GameMapDefinition, IIdentifiable
{
    /// <inheritdoc />
    public GameMapDefinition()
    {
        this.InitJoinCollections();
    }

    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="MonsterSpawns" />.
    /// </summary>
    public ICollection<MonsterSpawnArea> RawMonsterSpawns { get; } = new EntityFramework.List<MonsterSpawnArea>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.MonsterSpawnArea> MonsterSpawns => base.MonsterSpawns ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.MonsterSpawnArea, MonsterSpawnArea>(this.RawMonsterSpawns);

    /// <summary>
    /// Gets the raw collection of <see cref="EnterGates" />.
    /// </summary>
    public ICollection<EnterGate> RawEnterGates { get; } = new EntityFramework.List<EnterGate>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.EnterGate> EnterGates => base.EnterGates ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.EnterGate, EnterGate>(this.RawEnterGates);

    /// <summary>
    /// Gets the raw collection of <see cref="ExitGates" />.
    /// </summary>
    public ICollection<ExitGate> RawExitGates { get; } = new EntityFramework.List<ExitGate>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.ExitGate> ExitGates => base.ExitGates ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.ExitGate, ExitGate>(this.RawExitGates);

    /// <summary>
    /// Gets the raw collection of <see cref="MapRequirements" />.
    /// </summary>
    public ICollection<AttributeRequirement> RawMapRequirements { get; } = new EntityFramework.List<AttributeRequirement>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement> MapRequirements => base.MapRequirements ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.Items.AttributeRequirement, AttributeRequirement>(this.RawMapRequirements);

    /// <summary>
    /// Gets the raw collection of <see cref="CharacterPowerUpDefinitions" />.
    /// </summary>
    public ICollection<PowerUpDefinition> RawCharacterPowerUpDefinitions { get; } = new EntityFramework.List<PowerUpDefinition>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Attributes.PowerUpDefinition> CharacterPowerUpDefinitions => base.CharacterPowerUpDefinitions ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Attributes.PowerUpDefinition, PowerUpDefinition>(this.RawCharacterPowerUpDefinitions);

    /// <summary>
    /// Gets or sets the identifier of <see cref="SafezoneMap"/>.
    /// </summary>
    public Guid? SafezoneMapId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="SafezoneMap" />.
    /// </summary>
    [ForeignKey(nameof(SafezoneMapId))]
    public GameMapDefinition RawSafezoneMap
    {
        get => base.SafezoneMap as GameMapDefinition;
        set => base.SafezoneMap = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.GameMapDefinition SafezoneMap
    {
        get => base.SafezoneMap;set
        {
            base.SafezoneMap = value;
            this.SafezoneMapId = this.RawSafezoneMap?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="BattleZone"/>.
    /// </summary>
    public Guid? BattleZoneId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="BattleZone" />.
    /// </summary>
    [ForeignKey(nameof(BattleZoneId))]
    public BattleZoneDefinition RawBattleZone
    {
        get => base.BattleZone as BattleZoneDefinition;
        set => base.BattleZone = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.BattleZoneDefinition BattleZone
    {
        get => base.BattleZone;set
        {
            base.BattleZone = value;
            this.BattleZoneId = this.RawBattleZone?.Id;
        }
    }


    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        var baseObject = obj as IIdentifiable;
        if (baseObject != null)
        {
            return baseObject.Id == this.Id;
        }

        return base.Equals(obj);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    protected void InitJoinCollections()
    {
        this.DropItemGroups = new ManyToManyCollectionAdapter<MUnique.OpenMU.DataModel.Configuration.DropItemGroup, GameMapDefinitionDropItemGroup>(this.JoinedDropItemGroups, joinEntity => joinEntity.DropItemGroup, entity => new GameMapDefinitionDropItemGroup { GameMapDefinition = this, GameMapDefinitionId = this.Id, DropItemGroup = (DropItemGroup)entity, DropItemGroupId = ((DropItemGroup)entity).Id});
    }
}
