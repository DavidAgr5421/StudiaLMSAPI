namespace Studia.Domain.Activities;

// Individual y Grupal están implementadas end-to-end. Foro y Evaluación quedan
// definidas acá para que el ícono/selector ya las contemple, pero todavía no tienen
// comportamiento propio (llegan en una fase siguiente).
public enum ActivityKind
{
    Individual,
    Grupal,
    Foro,
    Evaluacion
}
