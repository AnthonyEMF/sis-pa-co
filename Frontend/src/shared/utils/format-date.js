export const formatDate = (isoDateString) => {
    const date = new Date(isoDateString);
    const options = {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
    };
    
    return date.toLocaleDateString('es-ES', options);
}

export const formatDateShort = (date) => {
    if (!date) return "";
    const parsedDate = new Date(date);
    if (isNaN(parsedDate)) return "Fecha inválida";
    return parsedDate.toLocaleDateString("es-ES", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
    });
};

export const getMonthName = (monthNumber) => {
    const months = [
      'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
      'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ];
  
    if (monthNumber < 1 || monthNumber > 12) {
      return 'Número de mes inválido';
    }
  
    return months[monthNumber - 1];
}
  
  