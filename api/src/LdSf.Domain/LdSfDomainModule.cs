using Volo.Abp.Modularity;

namespace LdSf;

[DependsOn(typeof(Volo.Abp.Domain.AbpDddDomainModule))]
public class LdSfDomainModule : AbpModule
{
}
