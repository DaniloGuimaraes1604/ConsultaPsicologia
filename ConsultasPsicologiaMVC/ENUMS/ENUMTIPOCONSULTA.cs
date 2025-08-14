using System.Runtime.Serialization;

namespace ConsultasPsicologiaMVC.ENUMS
{
    [DataContract]
    public enum ENUMTIPOCONSULTA
    {
        [EnumMember]
        VIRTUAL = 1,

        [EnumMember]
        PRESENCIAL = 2,
    }
}
