import React, {useState} from 'react';
import ErrorSummary, {errorMessage} from './ErrorSummary'

interface Props {
  title: string;
  errors: errorMessage[]
}

export default function LocationError({title, errors}: Props) {
  return(
    <>
    {errors.length > 0 &&
      <ErrorSummary errors={errors} />
    }
    </>
  )
}
