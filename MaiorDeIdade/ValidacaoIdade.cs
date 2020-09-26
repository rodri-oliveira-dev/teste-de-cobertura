using System;

namespace MaiorDeIdade
{
    public static class ValidacaoIdade
    {
        private const int MaiorDeIdade = 18;

        public static bool VerificaMaiorDeIdade(this DateTime dataNascimento)
        {
            int anoBase = DateTime.Today.Year - MaiorDeIdade;

            if (dataNascimento.Year < anoBase)
            {
                return true;
            }

            if (anoBase == dataNascimento.Year)
            {
                if (dataNascimento.Month < DateTime.Now.Month)
                {
                    return true;
                }

                if (dataNascimento.Month == DateTime.Now.Month)
                {
                    if (dataNascimento.Day <= DateTime.Now.Day)
                    {
                        return true;
                    }

                }
            }
            return false;
        }
    }
}
