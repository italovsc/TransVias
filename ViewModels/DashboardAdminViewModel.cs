namespace TransVias.ViewModels;

public class DashboardAdminViewModel
{
    public int Pendentes { get; set; }
    public int Aprovados { get; set; }
    public int Recusados { get; set; }

    public List<MotoristaResumoViewModel> Motoristas { get; set; } = [];
}