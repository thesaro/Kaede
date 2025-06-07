using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Bindables
{
    /// <summary>
    /// A simple bindable type that notifies when its value changes.
    /// </summary>
    /// <typeparam name="T">The type of value encapsulated by this bindable.</typeparam>
    public class Bindable<T>
    {
        private T? value;

        /// <summary>
        /// Raised when <see cref="Value"/> changes.
        /// </summary>
        public event Action<ValueChangedEvent<T>>? ValueChanged;

        /// <summary>
        /// The current value of this bindable, which may be absent (null).
        /// </summary>
        public T? Value
        {
            get => value;
            set
            {
                if (!Equals(this.value, value))
                {
                    var oldValue = this.value;
                    this.value = value;
                    ValueChanged?.Invoke(new ValueChangedEvent<T>(oldValue, value!));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bindable{T}"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value of the bindable.</param>
        public Bindable(T? initialValue = default)
        {
            value = initialValue;
        }

        /// <summary>
        /// Bind an action to <see cref="ValueChanged"/> to react to value updates.
        /// </summary>
        /// <param name="onChange">The action to perform when <see cref="Value"/> changes.</param>
        public void BindValueChanged(Action<ValueChangedEvent<T>> onChange)
            => ValueChanged += onChange;
    }

    public readonly struct ValueChangedEvent<T>
    {
        public readonly T? OldValue;
        public readonly T NewValue;

        public ValueChangedEvent(T? oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
