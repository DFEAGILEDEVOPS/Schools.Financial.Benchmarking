import {useEffect, useRef} from "react";

export default function useSetSafeTimeout() {
  const timers = useRef<number[]>([]);

  function setSafeInterval(callback: Function, delay: number): number {
    const newTimeout: number = setTimeout(callback, delay);
    timers.current.push(newTimeout);
    return newTimeout;
  }

  useEffect(() => {
    return () => {
      timers.current.forEach(t => {
        clearInterval(t);
      });
    }
  }, []);

  return setSafeInterval;
}