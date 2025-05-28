public class SkillData
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public int AnimId { get; private set; }
    public string EffectName { get; private set; }
    public float Cooldown { get; private set; }
    public int Damage { get; private set; }

    public SkillData(SkillDataDTO dto)
    {
        Id = dto.id;
        Name = dto.name;
        AnimId = dto.animId;
        EffectName = dto.effectName;
        Cooldown = dto.cooldown;
        Damage = dto.damage;
    }
}