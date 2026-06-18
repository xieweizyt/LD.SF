using Volo.Abp.Modularity;

namespace LdSf;

[DependsOn(typeof(LdSfApplicationContractsModule))]
public class LdSfApplicationModule : AbpModule
{
}
